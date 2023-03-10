using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reflection;

namespace Ostomachion.BlazorWebComponents;

public class CustomElementRegistrarComponent : ComponentBase
{
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    private ICustomElementRegistrar Registrar { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        foreach (var registration in Registrar.Registrations)
        {
            var localName = (string?)registration.Value
                .GetProperty(nameof(ICustomElement.LocalName), BindingFlags.Public | BindingFlags.Static)!
                .GetValue(null);

            await RegisterCustomElementWithJavaScriptAsync(registration.Key, localName);
        }
    }

    private async Task RegisterCustomElementWithJavaScriptAsync(string identifier, string? localName = null)
        => await JSRuntime.InvokeVoidAsync("window.blazorWebComponents.registerCustomElement", identifier, localName);
}
