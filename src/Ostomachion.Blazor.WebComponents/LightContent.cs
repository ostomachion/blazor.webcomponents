using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ostomachion.Blazor.WebComponents;

public class LightContent : ComponentBase
{
    [Parameter]
    public List<KeyValuePair<string, object?>> Slots { get; set; } = default!;

    public void Rerender() => StateHasChanged();

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
