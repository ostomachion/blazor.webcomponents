using Microsoft.AspNetCore.Components;

namespace Ostomachion.BlazorWebComponents.Demo.Demos.EditWord;

[WebComponent("edit-word")]
public partial class EditWord
{
    [Parameter]
    [EditorRequired]
    public string Value { get; set; } = null!;
}
