using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ostomachion.BlazorWebComponents.Generators;

[Generator]
public partial class WebComponentGenerator : IIncrementalGenerator
{
    // TODO: Create copies of .css files in wwwroot
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Do a simple filter for classes.
        var webComponentClasses = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null)!;

        // Combine the selected classes with the `Compilation`.
        var compilationAndClasses = context.CompilationProvider.Combine(webComponentClasses.Collect());

        // Generate the source using the compilation and classes.
        context.RegisterSourceOutput(compilationAndClasses,
            static (context, source) => GenerateSource(context, source.Right!));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };

    private static WebComponentSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        var attributeSyntax = GetAttribute("Ostomachion.BlazorWebComponents.WebComponentAttribute", context, classDeclarationSyntax.AttributeLists);
        var slotSyntaxes = GetSlotProperties(context);
        return attributeSyntax is null ? null : new WebComponentSyntax(classDeclarationSyntax, attributeSyntax, slotSyntaxes);
    }

    private static IEnumerable<SlotSyntax> GetSlotProperties(GeneratorSyntaxContext context)
    {
        Dictionary<string, SlotSyntax> value = new();
        HashSet<string> templates = new();
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        var properties = classDeclarationSyntax.Members.OfType<PropertyDeclarationSyntax>();
        foreach (var property in properties)
        {
            var name = property.Identifier.ToString();
            var attribute = GetAttribute("Ostomachion.BlazorWebComponents.SlotAttribute", context, property.AttributeLists);
            if (attribute is not null)
            {
                string typeString;
                var symbol = context.SemanticModel.GetTypeInfo(property.Type).Type;
                typeString = symbol!.ToDisplayString();

                value.Add(name, new SlotSyntax(property, attribute, typeString));
            }

            if (name.EndsWith("Template"))
            {
                var baseName = name.Substring(0, name.Length - "Template".Length);
                _ = templates.Add(baseName);
            }
        }

        foreach (var baseName in templates)
        {
            if (value.TryGetValue(baseName, out var slotSyntax))
            {
                slotSyntax.IsTemplated = true;
            }
        }

        return value.Values;
    }

    private static AttributeSyntax? GetAttribute(string fullName, GeneratorSyntaxContext context, SyntaxList<AttributeListSyntax> attributeLists)
    {
        foreach (var attributeListSyntax in attributeLists)
        {
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }

                if (attributeSymbol.ContainingType.ToDisplayString() == fullName)
                {
                    return attributeSyntax;
                }
            }
        }

        return null;
    }

    private static void GenerateSource(SourceProductionContext context, ImmutableArray<WebComponentSyntax> webComponentSyntaxes)
    {
        foreach (var item in webComponentSyntaxes)
        {
            var className = item.ClassDeclarationSyntax.Identifier.Text;

            var namespaceName = GetNamespace(item.ClassDeclarationSyntax);

            var styleSheetUrl = GetStyleSheetUrl(item.ClassDeclarationSyntax.SyntaxTree.FilePath);
            
            var builder = new StringBuilder()
                .AppendLine($$"""
                    #nullable enable

                    using System;
                    using Microsoft.AspNetCore.Components;
                    using Microsoft.AspNetCore.Components.Rendering;
                    using Ostomachion.BlazorWebComponents;

                    namespace {{namespaceName}};

                    partial class {{className}} : IWebComponent
                    {
                        private static string? _identifier;
                        public static string? Identifier
                        {
                            get => _identifier;
                            set
                            {
                                if (_identifier is not null)
                                {
                                    throw new InvalidOperationException("Identifier has already been set.");
                                }
                                _identifier = value;
                            }
                        }
                    
                        public static string? StylesheetUrl => {{(styleSheetUrl is null ? "null;" : $$"""_identifier is null ? null : $"{{styleSheetUrl}}";""")}}
                    """);

            if (item.SlotSyntaxes.Any())
            {
                _ = builder.Append("""

                        protected override void BuildRenderTreeSlots(RenderTreeBuilder builder)
                        {
                    """);

                var sequence = 0;
                foreach (var slotSyntax in item.SlotSyntaxes)
                {
                    _ = builder.AppendLine();

                    var type = slotSyntax.TypeString;
                    var name = slotSyntax.PropertyDeclarationSyntax.Identifier.ValueText;

                    // TODO: Support optional names. Default to property name.
                    var slotName = slotSyntax.AttributeSyntax.ArgumentList!.Arguments.First().GetText();

                    var rootElementName = slotSyntax.AttributeSyntax.ArgumentList!.Arguments
                        .FirstOrDefault(x => x.NormalizeWhitespace().GetText().ToString().StartsWith("RootElement = "))
                        ?.NormalizeWhitespace().GetText().ToString().Substring("RootElement = ".Length);

                    rootElementName ??= slotSyntax.IsTemplated || type == "Microsoft.AspNetCore.Components.RenderFragment" ? "\"div\"" : "\"span\"";

                    _ = builder.AppendLine($$"""
                                if ((object?){{name}} is not null && RenderedSlots.Contains("{{name}}"))
                                {
                                    builder.OpenElement({{sequence++}}, {{rootElementName}});
                                    builder.AddAttribute({{sequence++}}, "slot", {{slotName}});

                                    builder.AddAttribute({{sequence++}}, "xmlns:wc", GetType().Namespace);
                                    builder.AddAttribute({{sequence++}}, $"wc:{nameof({{name}})}");
                        """);

                    if (slotSyntax.IsTemplated)
                    {
                        _ = builder.AppendLine($$"""
                                        builder.AddAttribute({{sequence++}}, $"wc:{nameof({{name}}Template)}");

                                        if (this.{{name}}Template is null)
                                        {
                                            builder.AddContent({{sequence++}}, this.{{name}});
                                        }
                                        else
                                        {
                                            builder.AddContent({{sequence++}}, this.{{name}}Template, this.{{name}});
                                        }
                            """);
                    }
                    else
                    {
                        _ = builder.AppendLine($$"""
                                        builder.AddContent({{sequence++}}, this.{{name}});
                            """);
                    }

                    _ = builder.AppendLine($$"""
                                    builder.CloseElement();
                                }
                        """);
                }

                _ = builder.AppendLine($$"""
                        }
                    """);
            }

            foreach (var slotSyntax in item.SlotSyntaxes)
            {
                var name = slotSyntax.PropertyDeclarationSyntax.Identifier.ValueText;

                // TODO: Support optional names. Default to property name.
                var slotName = slotSyntax.AttributeSyntax.ArgumentList!.Arguments.First().GetText();

                _ = builder.AppendLine($$"""

                        private RenderFragment {{name}}Slot => (builder) =>
                        {
                            RenderedSlots.Add("{{name}}");
                            builder.OpenElement(0, "slot");
                            builder.AddAttribute(1, "name", {{slotName}});
                            builder.AddContent(2, {{slotName}});
                            builder.CloseElement();
                        };
                    """);
            }

            _ = builder.Append("}");

            context.AddSource($"{className}.g.cs", builder.ToString());
        }
    }

    private static string GetNamespace(BaseTypeDeclarationSyntax syntax)
    {
        var value = string.Empty;

        SyntaxNode? potentialNamespaceParent = syntax.Parent;

        while (potentialNamespaceParent is not null
            and not NamespaceDeclarationSyntax
            and not FileScopedNamespaceDeclarationSyntax)
        {
            potentialNamespaceParent = potentialNamespaceParent.Parent;
        }

        if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
        {
            value = namespaceParent.Name.ToString();

            while (true)
            {
                if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                {
                    break;
                }

                value = $"{namespaceParent.Name}.{value}";
                namespaceParent = parent;
            }
        }

        return value;
    }

    private static string? GetStyleSheetUrl(string csPath)
    {
        // TODO: I really want to use a URL and not an inline stylesheet.
        // I'm doing a dumb compromise with data-URLs until I figure out how to move the files in a good way.
        var razorPath = Path.ChangeExtension(csPath, null);
        var cssPath = Path.GetExtension(razorPath) == ".razor" && File.Exists(razorPath) ? razorPath + ".css" : csPath + ".css";
        return File.Exists(cssPath) ? $"data:text/css;base64,{Convert.ToBase64String(File.ReadAllBytes(cssPath))}" : null;
    }
}
