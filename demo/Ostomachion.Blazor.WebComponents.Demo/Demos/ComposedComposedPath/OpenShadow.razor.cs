using Microsoft.AspNetCore.Components;

namespace Ostomachion.Blazor.WebComponents.Demo.Demos.ComposedComposedPath;

public partial class OpenShadow
{
    public override ShadowRootMode ShadowRootMode => ShadowRootMode.Open;

    [Parameter]
    [EditorRequired]
    public string Text { get; set; } = default!;
}
