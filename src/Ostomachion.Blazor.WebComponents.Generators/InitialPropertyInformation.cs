using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ostomachion.Blazor.WebComponents.Generators;

internal record class InitialPropertyInformation
{
    public PropertyDeclarationSyntax PropertySyntax { get; set; } = null!;

    private InitialPropertyInformation() { }

    public static InitialPropertyInformation? Parse(PropertyDeclarationSyntax syntax, GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var slotAttributeInformation = syntax.AttributeLists
            .SelectMany(l => l.Attributes)
            .FirstOrDefault(x => x is not null);
        if (slotAttributeInformation is null)
        {
            return null;
        }

        return new InitialPropertyInformation
        {
            PropertySyntax = syntax,
        };
    }
}
