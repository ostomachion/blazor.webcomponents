using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Ostomachion.BlazorWebComponents.Demo.Demos.ExpandingList;

public partial class ExpandingList
{
    [Inject]
    public IJSRuntime JSRuntime { get; set; } = default!;

    public IJSObjectReference? Module { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./Demos/ExpandingList/ExpandingList.razor.js");
            await Module.InvokeVoidAsync("initialize", Host);
        }
    }
}
