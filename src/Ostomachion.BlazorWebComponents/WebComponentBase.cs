using System.ComponentModel;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ostomachion.BlazorWebComponents;

// TODO: See if there's a better design pattern to accomplish this?
public abstract class WebComponentBase<T> : WebComponentBaseImpl<T>
    where T : WebComponentBase<T>, IWebComponent
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected sealed override void BuildRenderTreeImpl(RenderTreeBuilder builder) => BuildRenderTree(builder);
    protected new virtual void BuildRenderTree(RenderTreeBuilder builder) => BaseBuildRenderTree(builder);
}
