using Microsoft.AspNetCore.Components;

namespace Ostomachion.Blazor.WebComponents.Demo.Demos.PopUpInfoBox;

[CustomElement("popup-info")]
public partial class PopupInfo : WebComponentBase
{
    [Parameter]
    [EditorRequired]
    public string Image { get; set; } = default!;

    [Parameter]
    [EditorRequired]
    public string Text { get; set; } = default!;
}
