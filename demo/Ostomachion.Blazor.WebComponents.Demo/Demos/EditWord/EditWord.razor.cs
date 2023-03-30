using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Ostomachion.Blazor.WebComponents.Demo.Demos.EditWord;

[CustomElement("edit-word")]
public partial class EditWord : WebComponentBase
{
    [Parameter]
    [EditorRequired]
    public string Value { get; set; } = null!;

    public bool Editing { get; set; }

    private ElementReference _input;
    private ElementReference _span;

    private async Task TextClickAsync(MouseEventArgs e)
    {
        await UpdateWidthAsync();
        Editing = true;
        await SelectAllAsync();
    }

    private void LabelBlur(FocusEventArgs e)
    {
        Editing = false;
    }

    private void FormSubmit(EventArgs e)
    {
        Editing = false;
    }

    private async Task SelectAllAsync() => await InvokeVoidAsync("selectAll", _input);
    private async Task UpdateWidthAsync() => await InvokeVoidAsync("updateWidth", _input, _span);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (Editing)
        {
            await _input.FocusAsync();
        }
    }
}