using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ostomachion.BlazorWebComponents.Generators;

public partial class WebComponentGenerator
{
    internal class WebComponentSyntax
    {
        public ClassDeclarationSyntax ClassDeclarationSyntax { get; set; }
        public AttributeSyntax AttributeSyntax { get; set; }
        public IEnumerable<SlotSyntax> SlotSyntaxes { get; set; }

        public WebComponentSyntax(ClassDeclarationSyntax classDeclarationSyntax, AttributeSyntax attributeSyntax, IEnumerable<SlotSyntax> slotSyntaxes)
        {
            ClassDeclarationSyntax = classDeclarationSyntax;
            AttributeSyntax = attributeSyntax;
            SlotSyntaxes = slotSyntaxes;
        }
    }
}
