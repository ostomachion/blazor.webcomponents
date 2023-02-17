using Microsoft.AspNetCore.Components;
using Ostomachion.BlazorWebComponents.Demo.Models;

namespace Ostomachion.BlazorWebComponents.Demo.WebComponents;

[WebComponent("element-details")]
public partial class ElementDetails
{
    [Parameter]
    [EditorRequired]
    public string Name { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public string Description { get; set; } = null!;

    [Parameter]
    public IEnumerable<AttributeDefinition> Attributes { get; set; } = Enumerable.Empty<AttributeDefinition>();
}