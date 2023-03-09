using Microsoft.AspNetCore.Components;

namespace Ostomachion.BlazorWebComponents.Demo.Demos.DefinedPseudoClass;

public partial class SimpleCustom
{
    [Parameter]
    [EditorRequired]
    public string Text { get; set; } = default!;
}
