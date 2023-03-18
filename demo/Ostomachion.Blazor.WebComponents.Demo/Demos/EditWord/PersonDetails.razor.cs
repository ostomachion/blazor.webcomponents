using Microsoft.AspNetCore.Components;

namespace Ostomachion.Blazor.WebComponents.Demo.Demos.EditWord;

[CustomElement("person-details")]
public partial class PersonDetails : WebComponentBase
{
    [Parameter]
    [EditorRequired]
    public RenderFragment PersonName { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public int PersonAge { get; set; }

    [Parameter]
    [EditorRequired]
    public string PersonOccupation { get; set; } = null!;
}
