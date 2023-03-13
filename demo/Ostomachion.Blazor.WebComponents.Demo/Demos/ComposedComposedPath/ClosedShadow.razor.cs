using Microsoft.AspNetCore.Components;

namespace Ostomachion.Blazor.WebComponents.Demo.Demos.ComposedComposedPath;

[CustomElement("closed-shadow")]
public partial class ClosedShadow : WebComponentBase
{
    public override ShadowRootMode ShadowRootMode => ShadowRootMode.Closed;

    [Parameter]
    [EditorRequired]
    public string Text { get; set; } = default!;
}
