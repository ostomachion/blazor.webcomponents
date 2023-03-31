using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ostomachion.Blazor.WebComponents;

/// <summary>
/// Represents the content in the light DOM of a <see cref="WebComponentBase"/> that will
/// be rendered in the component using <c>slot</c> elements. This class is managed by
/// <see cref="Slot{T}"/> and <see cref="WebComponentBase"/>.
/// </summary>
internal class SlottedLightContent : ComponentBase
{
    /// <summary>
    /// A collection of pairs of slot names together with the object to render.
    /// </summary>
    [Parameter]
    public List<ISlot> Slots { get; set; } = default!;

    /// <summary>
    /// Causes the component to rerender.
    /// </summary>
    public void Rerender() => StateHasChanged();

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        int sequence = 0;
        foreach (var slot in Slots.Where(x => x.RenderedValue is not null))
        {
            BuildSlotRenderTree(builder, slot, ref sequence);
        }
    }

    protected static void BuildSlotRenderTree(RenderTreeBuilder builder, ISlot slot, ref int sequence)
    {
        var hasName = !String.IsNullOrEmpty(slot.Name);

        if (hasName)
        {
            var elementName = slot.ElementName ?? (slot.RenderedValue is RenderFragment ? "div" : "span");
            builder.OpenElement(sequence++, elementName);
            builder.SetKey(slot.Name);
            builder.AddAttribute(sequence++, "slot", slot.Name);
        }

        if (slot.RenderedValue is RenderFragment renderFragment)
        {
            builder.OpenRegion(sequence++);
            renderFragment(builder);
            builder.CloseRegion();
        }
        else
        {
            builder.AddContent(sequence++, slot.RenderedValue);
        }

        if (hasName)
        {
            builder.CloseElement();
        }
    }
}
