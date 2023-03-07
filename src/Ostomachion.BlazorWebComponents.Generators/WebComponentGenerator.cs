using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ostomachion.BlazorWebComponents.Generators;

[Generator(LanguageNames.CSharp)]
public partial class WebComponentGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Get all .razor files.
        var files = context.AdditionalTextsProvider
            .Select((text, _) => text.Path)
            .Where(path => Path.GetExtension(path) == ".razor");

        // Get all classes that inherit WebComponentBase.
        var webComponents = context.SyntaxProvider.CreateSyntaxProvider(
            (n, _) => n is ClassDeclarationSyntax c &&
                c.BaseList is not null &&
                c.BaseList.Types.Any(t => t.Type is NameSyntax),
            (context, _) =>
            {
                var symbol = (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node)!;
                return (context.Node.SyntaxTree.FilePath, symbol.Name, symbol.ContainingNamespace, symbol.BaseType);
            })
            .Collect();

        //var files = context.SyntaxProvider.CreateSyntaxProvider(
        //    static (n, _) => n is 

        context.RegisterImplementationSourceOutput(webComponents, (context, webComponents) =>
        {
            context.AddSource("additional-files.g.txt", string.Join("\n", webComponents.Select(x => $"{Path.GetFileName(x.FilePath)} {x.Name} {x.ContainingNamespace} {x.BaseType}")));
        });
    }
}
