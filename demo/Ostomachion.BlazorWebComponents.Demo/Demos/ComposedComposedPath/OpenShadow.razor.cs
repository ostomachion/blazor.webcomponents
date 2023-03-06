using Microsoft.AspNetCore.Components;

namespace Ostomachion.BlazorWebComponents.Demo.Demos.ComposedComposedPath;

[WebComponent("open-shadow")]
public partial class OpenShadow
{
    public override ShadowRootMode ShadowRootMode => ShadowRootMode.Open;

    [Parameter]
    [EditorRequired]
    public string Text { get; set; } = default!;
}
