using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ostomachion.Blazor.WebComponents;

public class Slot<T> : ComponentBase
    where T : notnull
{
    [Parameter]
    [EditorRequired]
    public T? For { get; set; }

    [Parameter]
    public string? Name { get; set; }

    [CascadingParameter(Name = "Parent")]
    public WebComponentBase? Parent { get; set; }

    [Parameter]
    public RenderFragment<T>? Template { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object> Attributes { get; set; } = default!;

    public ElementReference ElementReference { get; private set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Parent is null)
        {
            throw new InvalidOperationException("The Slot component can only be used in components that inherit WebComponentBase.");
        }

        Parent.RegisterSlot(Name, For, Template);

        builder.OpenElement(0, "slot");
        builder.AddAttribute(1, "name", Name);
        builder.AddMultipleAttributes(2, Attributes);
        builder.AddElementReferenceCapture(3, x => ElementReference = x);
        builder.AddContent(4, ChildContent);
        builder.CloseElement();
    }
}
