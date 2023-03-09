using System.Collections.Immutable;
using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ostomachion.BlazorWebComponents;

// TODO: See if there's a better design pattern to accomplish this?
public abstract class WebComponentBase : WebComponentBaseImpl
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected sealed override void BuildRenderTreeImpl(RenderTreeBuilder builder) => BuildRenderTree(builder);
    protected new virtual void BuildRenderTree(RenderTreeBuilder builder) => BaseBuildRenderTree(builder);
}

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class WebComponentBaseImpl : CustomElementBase
{
    private static readonly Dictionary<Type, string?> _stylesheetMemo = new();
    protected string? GetStylesheet()
    {
        var type = GetType();
        if (!_stylesheetMemo.TryGetValue(type, out string? value))
        {
            value = (string?)type
                .GetProperty(nameof(IWebComponent.Stylesheet), BindingFlags.Public | BindingFlags.Static)!
                .GetValue(null);

            _stylesheetMemo.Add(type, value);
        }

        return value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected HashSet<string> RenderedSlots { get; } = new();

    protected virtual SlotLookup Slot => new(ImmutableDictionary<string, RenderFragment>.Empty);

    public virtual ShadowRootMode ShadowRootMode => ShadowRootMode.Open;

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected new virtual void BuildRenderTreeImpl(RenderTreeBuilder builder) => base.BuildRenderTree(builder);

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected new void BaseBuildRenderTree(RenderTreeBuilder builder) => base.BuildRenderTree(builder);

    protected sealed override void BuildRenderTree(RenderTreeBuilder builder)
    {
        RenderedSlots.Clear();

        builder.OpenElement(Line(), "template");
        builder.AddAttribute(Line(), "shadowrootmode", ShadowRootMode switch
        {
            ShadowRootMode.Open => "open",
            ShadowRootMode.Closed => "closed",
            _ => throw new InvalidOperationException("Unknown shadow root mode.")
        });

        var stylesheet = GetStylesheet();
        if (stylesheet is not null)
        {
            builder.OpenElement(Line(), "style");
            builder.AddContent(Line(), stylesheet);
            builder.CloseElement();
        }

        builder.OpenRegion(Line());
        BuildRenderTreeImpl(builder);
        builder.CloseRegion();

        builder.CloseElement();

        builder.OpenRegion(Line());
        BuildRenderTreeSlots(builder);
        builder.CloseRegion();

        static int Line([CallerLineNumber] int line = 0) => line;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void BuildRenderTreeSlots(RenderTreeBuilder builder) { }

    protected virtual bool IsTemplateDefined(object? property, string propertyName = null!) => false;
}
