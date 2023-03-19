using Microsoft.AspNetCore.Components;
using Ostomachion.Blazor.WebComponents.Demo.Models;

namespace Ostomachion.Blazor.WebComponents.Demo.Demos.ElementDetails;

[CustomElement("element-details")]
public partial class ElementDetails : WebComponentBase
{
    [Parameter]
    [EditorRequired]
    public string ElementName { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public RenderFragment Description { get; set; } = null!;

    [Parameter]
    public AttributeList? Attributes { get; set; }

    [Parameter]
    public RenderFragment<AttributeList?> AttributesTemplate { get; set; } = default!;
}
