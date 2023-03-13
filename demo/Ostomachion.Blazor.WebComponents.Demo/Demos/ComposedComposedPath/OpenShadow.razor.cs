using Microsoft.AspNetCore.Components;

namespace Ostomachion.Blazor.WebComponents.Demo.Demos.ComposedComposedPath;

[CustomElement("open-shadow")]
public partial class OpenShadow : WebComponentBase
{
    public override ShadowRootMode ShadowRootMode => ShadowRootMode.Open;

    [Parameter]
    [EditorRequired]
    public string Text { get; set; } = default!;
}
