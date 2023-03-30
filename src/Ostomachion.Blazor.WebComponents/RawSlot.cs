using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ostomachion.Blazor.WebComponents;

/// <summary>
/// Represents a <c>slot</c> element without the extra features of <see cref="Slot{T}"/>.
/// </summary>
public class RawSlot : ComponentBase
{
    /// <summary>
    /// The name of the slot.
    /// </summary>
    [Parameter]
    public string? Name { get; set; }

    /// <summary>
    /// Method invoked when the slot is changed.
    /// </summary>
    [Parameter]
    public Action<EventArgs>? OnSlotChange { get; set; } = default!;

    /// <summary>
    /// Method invoked when the slot is changed.
    /// </summary>
    [Parameter]
    public Func<EventArgs, Task>? OnSlotChangeAsync { get; set; } = default!;

    /// <summary>
    /// Additional attributes to add to the rendered <c>slot</c> element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object> Attributes { get; set; } = default!;

    /// <summary>
    /// Represents the contents of the rendered <c>slot</c> element.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// A reference to the rendered <c>slot</c> element.
    /// </summary>
    public ElementReference? ElementReference { get; private set; }

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "slot");
        builder.AddAttribute(1, "name", Name);

        if (OnSlotChangeAsync is not null && OnSlotChange is not null)
        {
            throw new InvalidOperationException($"Cannot set both {nameof(OnSlotChangeAsync)} and {nameof(OnSlotChange)}.");
        }

        if (OnSlotChangeAsync is not null)
        {
            builder.AddAttribute(2, "onslotchange", EventCallback.Factory.Create(this, OnSlotChangeAsync));
        }
        else if (OnSlotChange is not null)
        {
            builder.AddAttribute(2, "onslotchange", EventCallback.Factory.Create(this, OnSlotChange));
        }

        builder.AddMultipleAttributes(3, Attributes);
        builder.AddElementReferenceCapture(4, x => ElementReference = x);
        builder.AddContent(5, ChildContent);
        builder.CloseElement();
    }
}
