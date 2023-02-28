using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Ostomachion.BlazorWebComponents.Extensions;

namespace Ostomachion.BlazorWebComponents;

// TODO: See if there's a better design pattern to accomplish this?
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class WebComponentBaseImpl<T> : ComponentBase
    where T : WebComponentBase<T>, IWebComponent
{
    public virtual ShadowRootMode ShadowRootMode => ShadowRootMode.Open;

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected HashSet<string> RenderedSlots { get; } = new();

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void BuildRenderTreeImpl(RenderTreeBuilder builder) => base.BuildRenderTree(builder);

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected void BaseBuildRenderTree(RenderTreeBuilder builder) => base.BuildRenderTree(builder);
    protected sealed override void BuildRenderTree(RenderTreeBuilder builder)
    {
        RenderedSlots.Clear();

        builder.OpenElement(Line(), T.TagName);

        builder.OpenShadowRoot(Line(), ShadowRootMode);

        if (!String.IsNullOrWhiteSpace(T.TemplateCss))
        {
            builder.OpenElement(Line(), "style");
            builder.AddContent(Line(), T.TemplateCss);
            builder.CloseElement();
        }

        builder.OpenRegion(Line());
        BuildRenderTreeImpl(builder);
        builder.CloseRegion();

        builder.CloseShadowRoot();

        builder.OpenRegion(Line());
        BuildRenderTreeSlots(builder);
        builder.CloseRegion();

        builder.CloseElement();

        static int Line([CallerLineNumber] int line = 0) => line;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void BuildRenderTreeSlots(RenderTreeBuilder builder) { }
}
