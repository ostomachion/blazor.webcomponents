using Microsoft.AspNetCore.Components;

namespace Ostomachion.Blazor.WebComponents.Demo.Demos.PopUpInfoBox;

public partial class PopupInfo
{
    [Parameter]
    [EditorRequired]
    public string Image { get; set; } = default!;

    [Parameter]
    [EditorRequired]
    public string Text { get; set; } = default!;
}
