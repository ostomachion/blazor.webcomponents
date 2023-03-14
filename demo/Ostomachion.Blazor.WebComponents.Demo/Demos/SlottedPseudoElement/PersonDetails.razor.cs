using Microsoft.AspNetCore.Components;

namespace Ostomachion.Blazor.WebComponents.Demo.Demos.SlottedPseudoElement;

[CustomElement("slotted-person-details")]
public partial class PersonDetails : WebComponentBase
{
    [Parameter]
    [Slot("person-name", DefaultText = "NAME MISING")]
    public string? Name { get; set; }

    [Parameter]
    [Slot("person-age", DefaultText = "AGE MISSING")]
    public string? Age { get; set; }

    [Parameter]
    [Slot("person-occupation", DefaultText = "OCCUPATION MISSING")]
    public string? Occupation { get; set; }
}
