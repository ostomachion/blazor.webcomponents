using Microsoft.AspNetCore.Components;

namespace Ostomachion.BlazorWebComponents.Demo.Demos.DefinedPseudoClass;

[WebComponent("simple-custom")]
public partial class SimpleCustom
{
    [Parameter]
    [EditorRequired]
    public string Text { get; set; } = null!;
}
