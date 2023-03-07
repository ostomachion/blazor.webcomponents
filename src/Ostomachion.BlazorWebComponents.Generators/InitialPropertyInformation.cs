using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ostomachion.BlazorWebComponents.Generators;

internal record class InitialPropertyInformation
{
    public PropertyDeclarationSyntax PropertySyntax { get; set; } = null!;
    public InitialAttributeInformation AttributeInformation { get; set; } = null!;

    private InitialPropertyInformation() { }

    public static InitialPropertyInformation? Parse(PropertyDeclarationSyntax syntax, GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var attributeInformation = syntax.AttributeLists
            .SelectMany(l => l.Attributes)
            .Select(a => InitialAttributeInformation.Parse(a, context, cancellationToken))
            .FirstOrDefault(x => x is not null);
        if (attributeInformation is null)
        {
            return null;
        }

        return new InitialPropertyInformation
        {
            PropertySyntax = syntax,
            AttributeInformation = attributeInformation,
        };
    }
}
