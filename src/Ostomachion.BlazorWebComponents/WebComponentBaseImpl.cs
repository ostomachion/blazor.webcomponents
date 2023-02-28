using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using Ostomachion.BlazorWebComponents.Extensions;

namespace Ostomachion.BlazorWebComponents;

// TODO: See if there's a better design pattern to accomplish this?
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class WebComponentBaseImpl<T> : ComponentBase
    where T : WebComponentBase<T>, IWebComponent
{
    [Inject]
    protected virtual IJSRuntime JS { get; set; } = null!;
    protected IJSObjectReference? Module { get; set; }

    public virtual ShadowRootMode ShadowRootMode => ShadowRootMode.Open;

    public AttributeSet HostAttributes { get; } = new();

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
        builder.AddMultipleAttributes(Line(), HostAttributes!);

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


    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual Task OnInitializedAsyncImpl() => base.OnInitializedAsync();

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected Task BaseOnInitializedAsync() => base.OnInitializedAsync();
    protected sealed override async Task OnInitializedAsync()
    {
        // TODO: Maybe change where this is called so it's only called once.
        await JS.InvokeVoidAsync("window.blazorWebComponents.defineWebComponent", T.TagName);

        await OnInitializedAsyncImpl();
    }
}
