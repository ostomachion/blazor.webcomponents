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

    // determine the namespace the class/enum/struct is declared in, if any
    private static string GetNamespace(BaseTypeDeclarationSyntax syntax)
    {
        // If we don't have a namespace at all we'll return an empty string
        // This accounts for the "default namespace" case
        var value = string.Empty;

        // Get the containing syntax node for the type declaration
        // (could be a nested type, for example)
        SyntaxNode? potentialNamespaceParent = syntax.Parent;

        // Keep moving "out" of nested classes etc until we get to a namespace
        // or until we run out of parents
        while (potentialNamespaceParent is not null
            and not NamespaceDeclarationSyntax
            and not FileScopedNamespaceDeclarationSyntax)
        {
            potentialNamespaceParent = potentialNamespaceParent.Parent;
        }

        // Build up the final namespace by looping until we no longer have a namespace declaration
        if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
        {
            // We have a namespace. Use that as the type
            value = namespaceParent.Name.ToString();

            // Keep moving "out" of the namespace declarations until we 
            // run out of nested namespace declarations
            while (true)
            {
                if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                {
                    break;
                }

                // Add the outer namespace as a prefix to the final namespace
                value = $"{namespaceParent.Name}.{value}";
                namespaceParent = parent;
            }
        }

        // return the final namespace
        return value;
    }
}
