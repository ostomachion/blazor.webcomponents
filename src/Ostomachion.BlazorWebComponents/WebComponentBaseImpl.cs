using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Ostomachion.BlazorWebComponents;

// TODO: See if there's a better design pattern to accomplish this?
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class WebComponentBaseImpl<T> : ComponentBase
    where T : WebComponentBase<T>, IWebComponent
{
    protected string ModulePath => $"./_content/{Assembly.GetExecutingAssembly().GetName().Name}/blazor-web-components.js";

    [Inject]
    protected IJSRuntime JSRuntime { get; set; } = null!;

    protected IJSObjectReference? Module { get; set; }

    protected virtual void BuildRenderTreeImpl(RenderTreeBuilder builder) => base.BuildRenderTree(builder);
    protected void BaseBuildRenderTree(RenderTreeBuilder builder) => base.BuildRenderTree(builder);
    protected sealed override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, T.TagName);

        builder.OpenRegion(1);
        BuildRenderTreeSlots(builder);
        builder.CloseRegion();

        builder.OpenRegion(2);
        BuildRenderTreeImpl(builder);
        builder.CloseRegion();

        builder.CloseElement();
    }

    protected virtual void BuildRenderTreeSlots(RenderTreeBuilder builder) { }

    protected virtual Task OnInitializedImplAsync() => base.OnInitializedAsync();
    protected Task BaseOnInitializedAsync() => base.OnInitializedAsync();
    protected sealed override async Task OnInitializedAsync()
    {
        Module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath);
        await Module.InvokeVoidAsync("registerBlazorWebComponent", T.TagName, T.TemplateHtml, T.TemplateCss);
        await OnInitializedImplAsync();
    }
}
