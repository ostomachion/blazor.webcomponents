using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ostomachion.BlazorWebComponents;

public abstract class WebComponentBase : ComponentBase
{
    public WebComponentBase()
    {
        // Mimicking the constructor of ComponentBase, but adding to the render tree.
        // TODO: Try to find a better way to accomplish this.
        // Ideally: https://github.com/dotnet/roslyn/issues/57239
        SetPrivateMember("_renderFragment", (RenderFragment)(builder =>
        {
            SetPrivateMember("_hasPendingQueuedRender", false);
            SetPrivateMember("_hasNeverRendered", false);

            // All this to call this method instead of BuildRenderTree.
            BuildWebComponenetRenderTree(builder);
        }));

        void SetPrivateMember(string name, object? value) => typeof(ComponentBase)
            .GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)!
            .SetValue(this, value);
    }

    private void BuildWebComponenetRenderTree(RenderTreeBuilder builder)
    {
        // TODO:

        BuildRenderTree(builder);

        // TODO:
    }
}
