using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ostomachion.Blazor.WebComponents;

/// <summary>
/// Represents extra light content to be added to a web component without rendering it
/// as a full <see cref="Slot{T}"/>. Note that this can only be used inside  component
/// that inherits <see cref="WebComponentBase"/>.
/// </summary>
public class LightContent : ComponentBase
{
    /// <summary>
    /// The content to render in the light DOM.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// The parent <see cref="WebComponentBase"/> component.
    /// </summary>
    [CascadingParameter(Name = "Parent")]
    public WebComponentBase? Parent { get; set; }

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Parent is null)
        {
            throw new InvalidOperationException("The LightComponent component can only be used in components that inherit WebComponentBase.");
        }

        Parent.RegisterSlot(null, ChildContent, null);
    }
}
