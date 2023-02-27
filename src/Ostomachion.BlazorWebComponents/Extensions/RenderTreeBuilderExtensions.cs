using Microsoft.AspNetCore.Components.Rendering;

namespace Ostomachion.BlazorWebComponents.Extensions;
public static class RenderTreeBuilderExtensions
{
    public static void OpenShadowRoot(this RenderTreeBuilder builder, int sequence)
    {
        // Special element name that I hacked JS/Blazor to attach a shadow root
        // instead of creating an element when it comes across this name.
        // See blazor-hack.js for the code, if you dare.
        builder.OpenElement(sequence, "#shadow-root");
    }

    public static void CloseShadowRoot(this RenderTreeBuilder builder)
    {
        // For symmetry, even though it's interchangeable with CloseElement.
        builder.CloseElement();
    }
}
