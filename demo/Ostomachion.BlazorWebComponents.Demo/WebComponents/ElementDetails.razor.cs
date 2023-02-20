using Microsoft.AspNetCore.Components;
using Ostomachion.BlazorWebComponents.Demo.Models;

namespace Ostomachion.BlazorWebComponents.Demo.WebComponents;

[WebComponent("element-details")]
public partial class ElementDetails
{
    [Parameter]
    [EditorRequired]
    [Slot("element-name")]
    public string ElementName { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    [Slot("description")]
    public RenderFragment Description { get; set; } = null!;

    [Parameter]
    [Slot("attributes", IsTemplated = true)]
    public IEnumerable<AttributeDefinition> Attributes { get; set; } = Enumerable.Empty<AttributeDefinition>();
}
