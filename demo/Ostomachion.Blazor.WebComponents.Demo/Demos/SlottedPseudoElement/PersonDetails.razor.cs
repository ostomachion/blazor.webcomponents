using Microsoft.AspNetCore.Components;

namespace Ostomachion.Blazor.WebComponents.Demo.Demos.SlottedPseudoElement;

[CustomElement("slotted-person-details")]
public partial class PersonDetails : WebComponentBase
{
    [Parameter]
    public string? Name { get; set; }

    [Parameter]
    public string? Age { get; set; }

    [Parameter]
    public string? Occupation { get; set; }
}
