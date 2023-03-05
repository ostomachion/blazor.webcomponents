using Microsoft.AspNetCore.Components;

namespace Ostomachion.BlazorWebComponents.Demo.Demos.DefinedPseudoClass;

[WebComponent]
public partial class SimpleCustom
{
    [Parameter]
    [EditorRequired]
    public string Text { get; set; } = default!;
}
