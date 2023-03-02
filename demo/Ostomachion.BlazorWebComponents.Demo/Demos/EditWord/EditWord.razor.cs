using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Ostomachion.BlazorWebComponents.Demo.Demos.EditWord;

[WebComponent("edit-word")]
public partial class EditWord
{
    protected IJSObjectReference? Module { get; set; }

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

    private async Task FormSubmitAsync(EventArgs e)
    {
        await UpdateWidthAsync();
    }

    private async Task SelectAllAsync() => await (Module?.InvokeVoidAsync("selectAll", _input) ?? ValueTask.CompletedTask);
    private async Task UpdateWidthAsync() => await (Module?.InvokeVoidAsync("updateWidth", _input, _span) ?? ValueTask.CompletedTask);

    protected override async Task OnInitializedAsync()
    {
        Module = await JS.InvokeAsync<IJSObjectReference>("import", "./Demos/EditWord/EditWord.razor.js");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (Editing)
        {
            await _input.FocusAsync();
        }
    }
}