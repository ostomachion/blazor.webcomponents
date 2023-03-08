using Microsoft.AspNetCore.Components;

namespace Ostomachion.BlazorWebComponents.Demo.Demos.EditWord;

[CustomElement("person-details")]
public partial class PersonDetails
{
    [Parameter]
    [EditorRequired]
    [Slot("person-name", RootElement = "span")]
    public RenderFragment PersonName { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    [Slot("person-age")]
    public int PersonAge { get; set; }

    [Parameter]
    [EditorRequired]
    [Slot("person-occupation")]
    public string PersonOccupation { get; set; } = null!;
}
