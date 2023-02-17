using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Ostomachion.BlazorWebComponents;

public abstract class WebComponentBase<T> : ComponentBase where T : WebComponentBase<T>, IWebComponent
{
    [Inject] protected IJSRuntime JSRuntime { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await JSRuntime.InvokeVoidAsync("registerBlazorWebComponent", T.TagName, T.TemplateHtml, T.TemplateCss);
    }
}
