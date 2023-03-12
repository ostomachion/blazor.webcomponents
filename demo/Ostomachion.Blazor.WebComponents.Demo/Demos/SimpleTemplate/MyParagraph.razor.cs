using Microsoft.AspNetCore.Components;

namespace Ostomachion.Blazor.WebComponents.Demo.Demos.SimpleTemplate;

public partial class MyParagraph
{
    [Parameter]
    [EditorRequired]
    [Slot("my-text")]
    public RenderFragment ChildContent { get; set; } = default!;
}
