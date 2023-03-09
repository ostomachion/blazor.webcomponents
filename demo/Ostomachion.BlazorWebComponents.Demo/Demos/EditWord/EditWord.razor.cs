using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Ostomachion.BlazorWebComponents.Demo.Demos.EditWord;

public partial class EditWord
{
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = null!;

    private IJSObjectReference? Module { get; set; }

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

    private async Task SelectAllAsync() => await (Module?.InvokeVoidAsync("selectAll", _input) ?? ValueTask.CompletedTask);
    private async Task UpdateWidthAsync() => await (Module?.InvokeVoidAsync("updateWidth", _input, _span) ?? ValueTask.CompletedTask);

    protected override async Task OnInitializedAsync()
    {
        Module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./Demos/EditWord/EditWord.razor.js");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (Editing)
        {
            await _input.FocusAsync();
        }
    }
}