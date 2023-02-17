using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Ostomachion.BlazorWebComponents;

public abstract class WebComponentBase<T> : ComponentBase where T : WebComponentBase<T>, IWebComponent
{
    [Inject] protected IJSRuntime JSRuntime { get; set; } = null!;

    public WebComponentBase()
    {
        // Mimicking the constructor for ComponentBase.
        SetPrivateMember("_renderFragment", (RenderFragment)(builder =>
        {
            SetPrivateMember("_hasPendingQueuedRender", false);
            SetPrivateMember("_hasNeverRendered", false);
            BuildWebComponentRenderTree(builder);
        }));
    }

    private void SetPrivateMember(string name, object value)
    {
        typeof(ComponentBase)
            .GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)!
            .SetValue(this, value);
    }

    public void BuildWebComponentRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, T.TagName);
        builder.OpenRegion(1);
        BuildRenderTree(builder);
        builder.CloseRegion();
        builder.CloseElement();
    }

    protected override async Task OnInitializedAsync()
    {
        await JSRuntime.InvokeVoidAsync("registerBlazorWebComponent", T.TagName, T.TemplateHtml, T.TemplateCss);
    }
}
