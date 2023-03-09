using Microsoft.AspNetCore.Components;

namespace Ostomachion.BlazorWebComponents.Demo.Demos.ExpandingList;

public partial class ExpandingList : CustomElementBase
{
    [Parameter]
    public RenderFragment ChildContent { get; set; } = default !;
}