using System.Collections.Immutable;
using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ostomachion.BlazorWebComponents;

// TODO: See if there's a better design pattern to accomplish this?
/// <summary>
/// Represents a component wrapped in a custom element and encapsulated by a shadow DOM.
/// </summary>
public abstract class WebComponentBase : WebComponentBaseImpl
{
    /// <summary>
    /// Intended for internal use only. Use <see cref="BuildRenderTree(RenderTreeBuilder)"/> instead.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected sealed override void BuildRenderTreeImpl(RenderTreeBuilder builder) => BuildRenderTree(builder);

    /// <inheritdoc cref="ComponentBase.BuildRenderTree(RenderTreeBuilder)"/>
    protected new virtual void BuildRenderTree(RenderTreeBuilder builder) => BaseBuildRenderTree(builder);
}

/// <summary>
/// Internal use. Use <see cref="CustomElementBase"/> instead.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class WebComponentBaseImpl : CustomElementBase
{
    private static readonly Dictionary<Type, string?> _stylesheetMemo = new();

    /// <summary>
    /// Gets the value of <see cref="IWebComponent.Stylesheet"/> defined on the current type.
    /// </summary>
    /// <returns>The value of <see cref="IWebComponent.Stylesheet"/> defined on the current type.</returns>
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

    /// <summary>
    /// Intended for internal use only. A collection of the names of the slots that are currently rendered.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected HashSet<string> RenderedSlots { get; } = new();

    /// <summary>
    /// A <see cref="SlotLookup"/> to get the slot associated with a property marked with <see cref="SlotAttribute"/>.
    /// </summary>
    protected virtual SlotLookup Slot => new(ImmutableDictionary<string, RenderFragment>.Empty);

    /// <summary>
    /// Them encapsulation mode of the shadow root for this web component.
    /// </summary>
    public virtual ShadowRootMode ShadowRootMode => ShadowRootMode.Open;

    /// <summary>
    /// Intended for internal use only. Use <see cref="WebComponentBase.BuildRenderTree(RenderTreeBuilder)"/> instead.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected new virtual void BuildRenderTreeImpl(RenderTreeBuilder builder) => base.BuildRenderTree(builder);

    /// <summary>
    /// Intended for internal use only. Use <see cref="WebComponentBase.BuildRenderTree(RenderTreeBuilder)"/> instead.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected new void BaseBuildRenderTree(RenderTreeBuilder builder) => base.BuildRenderTree(builder);

    /// <inheritdoc/>
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

    /// <summary>
    /// Intended for internal use only. Called by <see cref="BuildRenderTree(RenderTreeBuilder)"/> to
    /// add the relevant <c>slot</c> elements to the light DOM.
    /// </summary>
    /// <param name="builder">A <see cref="RenderTreeBuilder"/> that will receive the render output.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void BuildRenderTreeSlots(RenderTreeBuilder builder) { }

    /// <summary>
    /// Determines whether or not a template parameter has been set for the specified property.
    /// </summary>
    /// <param name="property">The property to check.</param>
    /// <param name="propertyName">The name of the property to check.</param>
    /// <returns>
    /// <see langword="true"/> if a template parameter has been set for the specified property;
    /// <see langword="false"/> otherwise.
    /// </returns>
    protected virtual bool IsTemplateDefined(object? property, string propertyName = null!) => false;
}
