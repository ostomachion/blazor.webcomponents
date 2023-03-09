using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ostomachion.BlazorWebComponents;

// TODO: See if there's a better design pattern to accomplish this?
public abstract class CustomElementBase : CustomElementBaseImpl
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected sealed override void BuildRenderTreeImpl(RenderTreeBuilder builder) => BuildRenderTree(builder);
    protected new virtual void BuildRenderTree(RenderTreeBuilder builder) => BaseBuildRenderTree(builder);
}

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class CustomElementBaseImpl : ComponentBase
{
    private static readonly Dictionary<Type, string?> _identifierMemo = new();
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

    private static readonly Dictionary<Type, string?> _localNameMemo = new();
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

    public AttributeSet HostAttributes { get; } = new();

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void BuildRenderTreeImpl(RenderTreeBuilder builder) => base.BuildRenderTree(builder);

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected void BaseBuildRenderTree(RenderTreeBuilder builder) => base.BuildRenderTree(builder);
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

        builder.AddAttribute(Line(), "xmlns:ce", GetType().Namespace);
        builder.AddAttribute(Line(), $"ce:{GetType().Name}");

        builder.AddMultipleAttributes(Line(), HostAttributes!);

        builder.OpenRegion(Line());
        BuildRenderTreeImpl(builder);
        builder.CloseRegion();

        builder.CloseElement();

        static int Line([CallerLineNumber] int line = 0) => line;
    }
}
