using System.Collections.Immutable;
using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ostomachion.Blazor.WebComponents;

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
    private readonly List<KeyValuePair<string?, object?>> _slots = new();
    private SlottedLightContent? _slottedLightContent;

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
        _slots.Clear();

        // Root <template> element for declarative shadow DOM.
        builder.OpenElement(Line(), "template");
        builder.AddAttribute(Line(), "shadowrootmode", ShadowRootMode switch
        {
            ShadowRootMode.Open => "open",
            ShadowRootMode.Closed => "closed",
            _ => throw new InvalidOperationException("Unknown shadow root mode.")
        });

        // <style> element inside shadow root.
        var stylesheet = GetStylesheet();
        if (stylesheet is not null)
        {
            builder.OpenElement(Line(), "style");
            builder.AddContent(Line(), stylesheet);
            builder.CloseElement(); // style
        }

        // Add a CascadingValue named Parent to pass this component to any children,
        // specifically Slot and LightContent components.
        builder.OpenComponent<CascadingValue<WebComponentBase>>(Line());
        builder.AddAttribute(Line(), "Value", this as WebComponentBase);
        builder.AddAttribute(Line(), "Name", "Parent");

        // Build the render tree defined by CustomElementBase to the shadow root under the CascadingValue.
        builder.AddAttribute(Line(), "ChildContent", BuildRenderTreeImpl);

        builder.CloseComponent(); // CascadingValue

        builder.CloseElement(); // template

        // Add a SlottedLightContent component for Slot and LightContent components to interact with
        // through the Parent CascadingValue and the RegisterSlot method.
        builder.OpenComponent<SlottedLightContent>(Line());
        builder.AddAttribute(Line(), nameof(SlottedLightContent.Slots), _slots);
        builder.AddComponentReferenceCapture(Line(), x => _slottedLightContent = (SlottedLightContent)x);
        builder.CloseComponent(); // SlottedLightContent

        static int Line([CallerLineNumber] int line = 0) => line;
    }

    /// <summary>
    /// Registers a <see cref="Slot{T}"/> or <see cref="LightContent"/> component so that this
    /// component can render the appropriate light content.
    /// </summary>
    /// <typeparam name="T">The type of the slotted content.</typeparam>
    /// <param name="name">The name of the slot.</param>
    /// <param name="value">The slotted content.</param>
    /// <param name="template">A template to render the slotted content.</param>
    /// <exception cref="InvalidOperationException"></exception>
    internal void RegisterSlot<T>(string? name, T? value, RenderFragment<T>? template)
    {
        object? renderedValue = value is not null && template is not null ? template(value) : value;

        if (_slots.Any(x => x.Key == name))
        {
            throw new InvalidOperationException(name is null
                ? "Only one LightComponent and/or Slot without a name can be added to a component."
                : $"A Slot with name {name} has already been added to the component.");
        }

        _slots.Add(new(name, renderedValue));
        _slottedLightContent?.Rerender();
    }
}
