using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Ostomachion.BlazorWebComponents.Generators;

[Generator]
public class WebComponentGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Do a simple filter for classes.
        var webComponentClasses = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s), // select enums with attributes
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx)) // sect the enum with the [EnumExtensions] attribute
            .Where(static m => m is not null)!; // filter out attributed enums that we don't care about

        // Combine the selected classes with the `Compilation`.
        var compilationAndClasses = context.CompilationProvider.Combine(webComponentClasses.Collect());

        // Generate the source using the compilation and classes.
        context.RegisterSourceOutput(compilationAndClasses,
            static (spc, source) => GenerateSource(spc, source.Right!));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };

    private static (ClassDeclarationSyntax Class, AttributeSyntax Attribute)? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

        foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
        {
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }

                var fullName = attributeSymbol.ContainingType.ToDisplayString();

                if (fullName == "Ostomachion.BlazorWebComponents.WebComponentAttribute")
                {
                    return (classDeclarationSyntax, attributeSyntax);
                }
            }
        }

        return null;
    }

    private static void GenerateSource(SourceProductionContext context, ImmutableArray<(ClassDeclarationSyntax Class, AttributeSyntax Attribute)?> classes)
    {
        foreach (var item in classes.Select(x => x!.Value))
        {
            var className = item.Class.Identifier.Text;

            var namespaceName = GetNamespace(item.Class);

            var tagNameExpression = item.Attribute.ArgumentList!.Arguments.First()
                .Expression.NormalizeWhitespace().ToFullString();

            var classContent = $$"""
                using Microsoft.AspNetCore.Components;
                using Ostomachion.BlazorWebComponents;

                namespace {{namespaceName}};

                partial class {{className}} : IWebComponent
                {
                    [Parameter]
                    public ShadowRootMode ShadowRootMode { get; set; }

                    public static string TagName => {{tagNameExpression}};

                    public static string TemplateHtml => "<h1>Hello, world!</h1>";

                    public static string? TemplateCss => "* { background: red; }";
                }
                """;

            context.AddSource($"{className}.g.cs", classContent);
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
}
