using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Ostomachion.Blazor.WebComponents.Demo.Models;

namespace Ostomachion.Blazor.WebComponents.Demo.Demos.SlotChange;

[CustomElement("summary-display")]
public partial class SummaryDisplay : WebComponentBase
{
    [Parameter]
    public IEnumerable<SummaryDisplayItem> Items { get; set; } = default!;

    public SummaryDisplayItem? Choice { get; set; }

    private void ItemClick(SummaryDisplayItem item)
    {
        Choice = item;
    }

    private async Task SlotChangeAsync(EventArgs _) => await InvokeJSVoidAsync("slotChange");
}
