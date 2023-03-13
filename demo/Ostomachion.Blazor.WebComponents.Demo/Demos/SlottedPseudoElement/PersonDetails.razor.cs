using Microsoft.AspNetCore.Components;

namespace Ostomachion.Blazor.WebComponents.Demo.Demos.SlottedPseudoElement;

[CustomElement("slotted-person-details")]
public partial class PersonDetails : WebComponentBase
{
    [Parameter]
    [Slot("person-name")]
    public string? Name { get; set; }

    [Parameter]
    [Slot("person-age")]
    public string? Age { get; set; }

    [Parameter]
    [Slot("person-occupation")]
    public string? Occupation { get; set; }
}
