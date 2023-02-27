using Microsoft.AspNetCore.Components.Rendering;

namespace Ostomachion.BlazorWebComponents.Extensions;
public static class RenderTreeBuilderExtensions
{
    public static void OpenShadowRoot(this RenderTreeBuilder builder, int sequence, ShadowRootMode mode)
    {
        // I hacked JS/Blazor to attach a shadow root instead of creating an element
        // when it comes across these special names.
        // See blazor-hack.js for the code, if you dare.
        builder.OpenElement(sequence, mode switch
        {
            ShadowRootMode.Open => "#shadow-root (open)",
            ShadowRootMode.Closed => "#shadow-root (closed)",
            _ => throw new ArgumentException("Unknown shadow root mode.", nameof(mode))
        });
    }

    public static void CloseShadowRoot(this RenderTreeBuilder builder)
    {
        // For symmetry, even though it's interchangeable with CloseElement.
        builder.CloseElement();
    }
}
