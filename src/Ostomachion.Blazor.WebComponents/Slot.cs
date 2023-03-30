using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ostomachion.Blazor.WebComponents;

/// <summary>
/// Represents a <c>slot</c> element inside a shadow DOM and handles adding the slotted
/// light content to the parent component. Note that this can only be used inside a
/// component that inherits <see cref="WebComponentBase"/>.
/// </summary>
/// <typeparam name="T">The type of the value to be added to the light DOM.</typeparam>
public class Slot<T> : ComponentBase
    where T : notnull
{
    /// <summary>
    /// The object to be added to the light DOM.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public T? For { get; set; }

    /// <summary>
    /// The name of the slot. Only one unnamed slot can be rendered on a component at a
    /// time, and each rendered named slot must have a unique name on the component.
    /// </summary>
    [Parameter]
    public string? Name { get; set; }

    /// <summary>
    /// Method invoked when the slot is changed.
    /// </summary>
    [Parameter]
    public Action<EventArgs>? OnSlotChange { get; set; } = default!;

    /// <summary>
    /// Additional attributes to add to the rendered <c>slot</c> element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object> Attributes { get; set; } = default!;

    /// <summary>
    /// An optional <see cref="RenderFragment{TValue}"/> to apply to the value of
    /// <see cref="For"/> when rendering it in the light DOM.
    /// </summary>
    [Parameter]
    public RenderFragment<T>? Template { get; set; }

    /// <summary>
    /// Represents the contents of the rendered <c>slot</c> element. This will be rendered
    /// instead of <see cref="For"/> if <see cref="For"/> is <see langword="null"/>.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// The parent <see cref="WebComponentBase"/> component.
    /// </summary>
    [CascadingParameter(Name = "Parent")]
    public WebComponentBase? Parent { get; set; }

    /// <summary>
    /// A reference to the rendered <c>slot</c> element.
    /// </summary>
    public ElementReference? ElementReference { get; private set; }

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Parent is null)
        {
            throw new InvalidOperationException("The Slot component can only be used in components that inherit WebComponentBase.");
        }

        Parent.RegisterSlot(Name, For, Template);

        builder.OpenElement(0, "slot");
        builder.AddAttribute(1, "name", Name);
        if (OnSlotChange is not null)
        {
            builder.AddAttribute(2, "onslotchange", EventCallback.Factory.Create(this, OnSlotChange));
        }

        builder.AddMultipleAttributes(3, Attributes);
        builder.AddElementReferenceCapture(4, x => ElementReference = x);
        builder.AddContent(5, ChildContent);
        builder.CloseElement();
    }
}
