using Microsoft.AspNetCore.Components;

namespace Ostomachion.Blazor.WebComponents.Demo.Demos.SimpleTemplate;

[CustomElement("my-paragraph")]
public partial class MyParagraph : WebComponentBase
{
    [Parameter]
    [EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;
}
