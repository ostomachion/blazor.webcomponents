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
    public List<KeyValuePair<string, object?>> Slots { get; set; } = default!;

    /// <summary>
    /// Causes the component to rerender.
    /// </summary>
    public void Rerender() => StateHasChanged();

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        int sequence = 0;
        foreach (var slot in Slots.Where(x => x.Value is not null))
        {
            BuildSlotRenderTree(builder, slot.Key, slot.Value, ref sequence);
        }
    }

    protected static void BuildSlotRenderTree(RenderTreeBuilder builder, string name, object? value, ref int sequence)
    {
        var hasName = !String.IsNullOrEmpty(name);

        if (hasName)
        {
            // TODO: Set element name.
            builder.OpenElement(sequence++, "span");
            builder.SetKey(name);
            builder.AddAttribute(sequence++, "slot", name);
        }

        if (value is RenderFragment renderFragment)
        {
            builder.OpenRegion(sequence++);
            renderFragment(builder);
            builder.CloseRegion();
        }
        else
        {
            builder.AddContent(sequence++, value);
        }

        if (hasName)
        {
            builder.CloseElement();
        }
    }
}
