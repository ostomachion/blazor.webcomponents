using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Ostomachion.BlazorWebComponents;

// TODO: See if there's a better design pattern to accomplish this?
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class WebComponentBaseImpl : ComponentBase
{
    private string? _identifier;
    private string? _stylesheetUrl;

    private string? GetIdentifier()
    {
        _identifier ??= (string?)GetType().GetProperty(nameof(IWebComponent.Identifier), BindingFlags.Public | BindingFlags.Static)!.GetValue(null);
        return _identifier;
    }

    private string? GetStylesheetUrl()
    {
        _stylesheetUrl ??= (string?)GetType().GetProperty(nameof(IWebComponent.StylesheetUrl), BindingFlags.Public | BindingFlags.Static)!.GetValue(null);
        return _stylesheetUrl;
    }

    [Inject]
    protected virtual IJSRuntime JSRuntime { get; set; } = null!;

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
        var identifier = GetIdentifier() ?? throw new InvalidOperationException("The web component's identifier has not been set.");
        var stylesheetUrl = GetStylesheetUrl();

        RenderedSlots.Clear();

        builder.OpenElement(Line(), identifier);
        builder.AddMultipleAttributes(Line(), HostAttributes!);

        builder.OpenElement(Line(), "template");
        builder.AddAttribute(Line(), "shadowrootmode", ShadowRootMode switch
        {
            ShadowRootMode.Open => "open",
            ShadowRootMode.Closed => "closed",
            _ => throw new InvalidOperationException("Unknown shadow root mode.")
        });

        if (stylesheetUrl is not null)
        {
            builder.OpenElement(Line(), "link");
            builder.AddAttribute(Line(), "rel", "stylesheet");
            builder.AddAttribute(Line(), "href", stylesheetUrl);
            builder.CloseElement();
        }

        builder.OpenRegion(Line());
        BuildRenderTreeImpl(builder);
        builder.CloseRegion();

        builder.CloseElement();

        builder.OpenRegion(Line());
        BuildRenderTreeSlots(builder);
        builder.CloseRegion();

        builder.CloseElement();

        static int Line([CallerLineNumber] int line = 0) => line;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void BuildRenderTreeSlots(RenderTreeBuilder builder) { }
}
