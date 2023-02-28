using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Ostomachion.BlazorWebComponents.Demo.Demos.ComposedComposedPath;

public partial class Index
{
    [Inject]
    protected IJSRuntime JS { get; set; } = null!;

    protected IJSObjectReference? Module { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Module = await JS.InvokeAsync<IJSObjectReference>("import", "./Demos/ComposedComposedPath/Index.razor.js");

        await Module.InvokeVoidAsync("initialize");
    }
}