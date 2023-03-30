using Microsoft.AspNetCore.Components;
using Ostomachion.Blazor.WebComponents.Demo.Models;

namespace Ostomachion.Blazor.WebComponents.Demo.Demos.SlotChange;

[CustomElement("summary-display")]
public partial class SummaryDisplay : WebComponentBase
{
    [Parameter]
    public IEnumerable<SummaryDisplayItem> Items { get; set; } = default!;

    public SummaryDisplay? Choice { get; set; }

    public async Task ItemClickAsync(ElementReference elementReference)
    {
        await InvokeVoidAsync("handleClick");
    }

    public void SlotChange(EventArgs e)
    {

    }
}
