using Microsoft.AspNetCore.Components;

namespace Ostomachion.Blazor.WebComponents.Demo.Demos.DefinedPseudoClass;

[CustomElement("simple-custom")]
public partial class SimpleCustom : WebComponentBase
{
    [Parameter]
    [EditorRequired]
    public string Text { get; set; } = default!;
}
