using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ostomachion.Blazor.WebComponents;

// TODO: See if there's a better design pattern to accomplish this?
/// <summary>
/// Represents a component wrapped in a custom element.
/// </summary>
public abstract class CustomElementBase : CustomElementBaseImpl
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
public abstract class CustomElementBaseImpl : ComponentBase
{
    private static readonly Dictionary<Type, string?> _identifierMemo = new();
    private static readonly Dictionary<Type, string?> _localNameMemo = new();

    /// <summary>
    /// Gets the value of <see cref="ICustomElement.Identifier"/> defined on the current type.
    /// </summary>
    /// <returns>The value of <see cref="ICustomElement.Identifier"/> defined on the current type.</returns>
    protected string? GetIdentifier()
    {
        var type = GetType();
        if (!_identifierMemo.TryGetValue(type, out string? value))
        {
            value = (string?)type
                .GetProperty(nameof(ICustomElement.Identifier), BindingFlags.Public | BindingFlags.Static)!
                .GetValue(null);

            _identifierMemo.Add(type, value);
        }

        return value;
    }

    /// <summary>
    /// Gets the value of <see cref="ICustomElement.LocalName"/> defined on the current type.
    /// </summary>
    /// <returns>The value of <see cref="ICustomElement.LocalName"/> defined on the current type.</returns>
    protected string? GetLocalName()
    {
        var type = GetType();
        if (!_localNameMemo.TryGetValue(type, out string? value))
        {
            value = (string?)type
                .GetProperty(nameof(ICustomElement.LocalName), BindingFlags.Public | BindingFlags.Static)!
                .GetValue(null);

            _localNameMemo.Add(type, value);
        }

        return value;
    }

    /// <summary>
    /// An <see cref="ElementReference"/> to the generated custom element that wraps this component.
    /// </summary>
    public ElementReference? Host { get; private set; }

    /// <summary>
    /// Allows managing the attributes set on the generated custom element that wraps this component.
    /// </summary>
    public AttributeSet HostAttributes { get; } = new();

    /// <summary>
    /// Intended for internal use only. Use <see cref="CustomElementBase.BuildRenderTree(RenderTreeBuilder)"/> instead.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void BuildRenderTreeImpl(RenderTreeBuilder builder) => base.BuildRenderTree(builder);

    /// <summary>
    /// Intended for internal use only. Use <see cref="CustomElementBase.BuildRenderTree(RenderTreeBuilder)"/> instead.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected void BaseBuildRenderTree(RenderTreeBuilder builder) => base.BuildRenderTree(builder);

    /// <summary>
    /// Intended for internal use only. Use <see cref="CustomElementBase.BuildRenderTree(RenderTreeBuilder)"/> instead.
    /// </summary>
    protected sealed override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var identifier = GetIdentifier() ?? throw new InvalidOperationException("The web component's identifier has not been set.");
        var localName = GetLocalName();

        if (localName is null)
        {
            builder.OpenElement(Line(), identifier);
        }
        else
        {
            builder.OpenElement(Line(), localName);
            builder.AddAttribute(Line(), "is", identifier);
        }

        builder.AddAttribute(Line(), $"{GetType().Namespace?.ToLowerInvariant()}|{GetType().Name.ToLowerInvariant()}");

        builder.AddMultipleAttributes(Line(), HostAttributes!);

        builder.AddElementReferenceCapture(Line(), el => Host = el);

        builder.OpenRegion(Line());
        BuildRenderTreeImpl(builder);
        builder.CloseRegion();

        builder.CloseElement();

        static int Line([CallerLineNumber] int line = 0) => line;
    }
}
