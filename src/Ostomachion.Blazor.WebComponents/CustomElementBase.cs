using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

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
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

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

    /// <inheritdoc cref="JSObjectReferenceExtensions.InvokeVoidAsync(IJSObjectReference, string, object?[]?)"/>
    public async ValueTask InvokeJSVoidAsync(string identifier, params object?[]? args)
        => await JSRuntime.InvokeVoidAsync("window.blazorWebComponents.invokeMethod", Host, identifier, args);

    /// <inheritdoc cref="JSObjectReferenceExtensions.InvokeVoidAsync(IJSObjectReference, string, CancellationToken, object?[]?)"/>
    public async ValueTask InvokeJSVoidAsync(string identifier, CancellationToken cancellationToken, params object?[]? args)
        => await JSRuntime.InvokeVoidAsync("window.blazorWebComponents.invokeMethod", cancellationToken, Host, identifier, args);

    /// <inheritdoc cref="JSObjectReferenceExtensions.InvokeVoidAsync(IJSObjectReference, string, TimeSpan, object?[]?)"/>
    public async ValueTask InvokeJSVoidAsync(string identifier, TimeSpan timeout, params object?[]? args)
        => await JSRuntime.InvokeVoidAsync("window.blazorWebComponents.invokeMethod", timeout, Host, identifier, args);

    /// <inheritdoc cref="JSObjectReferenceExtensions.InvokeAsync{TValue}(IJSObjectReference, string, object?[]?)"/>
    public async ValueTask<TValue> InvokeJSAsync<TValue>(string identifier, params object?[]? args)
        => await JSRuntime.InvokeAsync<TValue>("window.blazorWebComponents.invokeMethod", Host, identifier, args);

    /// <inheritdoc cref="JSObjectReferenceExtensions.InvokeAsync{TValue}(IJSObjectReference, string, CancellationToken, object?[]?)"/>
    public async ValueTask<TValue> InvokeJSAsync<TValue>(string identifier, CancellationToken cancellationToken, params object?[]? args)
        => await JSRuntime.InvokeAsync<TValue>("window.blazorWebComponents.invokeMethod", cancellationToken, Host, identifier, args);

    /// <inheritdoc cref="JSObjectReferenceExtensions.InvokeAsync{TValue}(IJSObjectReference, string, TimeSpan, object?[]?)"/>
    public async ValueTask<TValue> InvokeJSAsync<TValue>(string identifier, TimeSpan timeout, params object?[]? args)
        => await JSRuntime.InvokeAsync<TValue>("window.blazorWebComponents.invokeMethod", timeout, Host, identifier, args);

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

        // Custom element, either an autonomous element or a customized built-in element.
        if (localName is null)
        {
            builder.OpenElement(Line(), identifier);
        }
        else
        {
            builder.OpenElement(Line(), localName);
            builder.AddAttribute(Line(), "is", identifier);
        }

        // Special namespaced attribute for CSS selectors.
        builder.AddAttribute(Line(), $"{GetType().Namespace?.ToLowerInvariant()}|{GetType().Name.ToLowerInvariant()}");

        builder.AddMultipleAttributes(Line(), HostAttributes!);

        builder.AddElementReferenceCapture(Line(), el => Host = el);

        // Add the user-defined content, e.g. from the .razor file.
        builder.OpenRegion(Line());
        BuildRenderTreeImpl(builder);
        builder.CloseRegion();

        builder.CloseElement();

        static int Line([CallerLineNumber] int line = 0) => line;
    }
}
