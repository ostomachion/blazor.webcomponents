using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ostomachion.BlazorWebComponents.Generators;

internal class SlotSyntax
{
    public PropertyDeclarationSyntax PropertyDeclarationSyntax { get; set; }
    public AttributeSyntax AttributeSyntax { get; set; }
    public string TypeString { get; set; }

    public SlotSyntax(PropertyDeclarationSyntax propertyDeclarationSyntax, AttributeSyntax attributeSyntax, string typeString)
    {
        PropertyDeclarationSyntax = propertyDeclarationSyntax;
        AttributeSyntax = attributeSyntax;
        TypeString = typeString;
    }
}