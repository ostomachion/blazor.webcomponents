using Microsoft.AspNetCore.Components;

namespace Ostomachion.Blazor.WebComponents.Demo.Demos.ComposedComposedPath;

public partial class ClosedShadow
{
    public override ShadowRootMode ShadowRootMode => ShadowRootMode.Closed;

    [Parameter]
    [EditorRequired]
    public string Text { get; set; } = default!;
}
