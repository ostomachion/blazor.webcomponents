using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Ostomachion.BlazorWebComponents.Demo.Demos.EditWord;

[WebComponent("edit-word")]
public partial class EditWord
{
    [Parameter]
    [EditorRequired]
    public string Value { get; set; } = null!;

    public bool Editing { get; set; }

    private ElementReference _input;

    public void Click(MouseEventArgs e)
    {
        Editing = true;
    }

    public void Blur(FocusEventArgs e)
    {
        Editing = false;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (Editing)
        {
            await _input.FocusAsync();
        }
    }
}