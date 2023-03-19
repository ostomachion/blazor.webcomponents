using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ostomachion.Blazor.WebComponents;

/// <summary>
/// Represents the content in the light DOM of a <see cref="WebComponentBase"/> that will
/// be rendered in the component using <c>slot</c> elements. This class is managed by
/// <see cref="Slot{T}"/> and <see cref="WebComponentBase"/>.
/// </summary>
internal class LightContent : ComponentBase
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
        var n = 3;
        for (int i = 0; i < Slots.Count; i++)
        {
            var slot = Slots[i];
            if (slot.Value is null)
            {
                continue;
            }

            // TODO: Set element name.
            builder.OpenElement(n * i, "span");
            builder.SetKey(slot.Key);
            if (!String.IsNullOrEmpty(slot.Key))
            {
                builder.AddAttribute(n * i + 1, "slot", slot.Key);
            }

            if (slot.Value is RenderFragment rf)
            {
                builder.OpenRegion(n * i + 2);
                rf(builder);
                builder.CloseRegion();
            }
            else
            {
                builder.AddContent(n * i + 2, slot.Value);
            }

            builder.CloseElement();
        }
    }
}
