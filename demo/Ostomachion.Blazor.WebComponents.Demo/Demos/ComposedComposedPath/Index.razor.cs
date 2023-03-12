using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Ostomachion.Blazor.WebComponents.Demo.Demos.ComposedComposedPath;

public partial class Index
{
    [Inject]
    protected IJSRuntime JSRuntime { get; set; } = default!;

    protected IJSObjectReference? Module { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./Demos/ComposedComposedPath/Index.razor.js");

        await Module.InvokeVoidAsync("initialize");
    }
}