using Microsoft.AspNetCore.Components;

namespace Ostomachion.Blazor.WebComponents.Demo.Demos.ExpandingList;

[CustomElement("expanding-list", Extends = "ul")]
public partial class ExpandingList : CustomElementBase
{
    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InvokeVoidAsync("initialize");
        }
    }
}
