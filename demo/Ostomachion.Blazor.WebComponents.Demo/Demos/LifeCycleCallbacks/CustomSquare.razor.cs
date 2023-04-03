using Microsoft.AspNetCore.Components;
using System.Drawing;

namespace Ostomachion.Blazor.WebComponents.Demo.Demos.LifeCycleCallbacks;

[CustomElement("custom-square")]
public partial class CustomSquare : WebComponentBase
{
    [Parameter]
    [EditorRequired]
    public int Length { get; set; }

    [Parameter]
    [EditorRequired]
    public Color Color { get; set; }
}
