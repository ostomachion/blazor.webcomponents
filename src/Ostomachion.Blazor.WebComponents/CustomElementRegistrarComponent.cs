using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reflection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Ostomachion.Blazor.WebComponents;

/// <summary>
/// A special component that is responsible for defining custom elements that have been registered through
/// <see cref="CustomElementRegistrar"/> with the JavaScript framework. This is added as a root component
/// to <c>head::after</c> by <see cref="ServiceCollectionExtensions.AddBlazorWebComponents(IServiceCollection,
/// Action{CustomElementRegistrar})"/>.
/// </summary>
public sealed class CustomElementRegistrarComponent : ComponentBase
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    private ICustomElementRegistrar Registrar { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            foreach (var registration in Registrar.Registrations)
            {
                var localName = (string?)registration.Value
                    .GetProperty(nameof(ICustomElement.LocalName), BindingFlags.Public | BindingFlags.Static)!
                    .GetValue(null);

                var modulePath = (string?)registration.Value
                    .GetProperty(nameof(ICustomElement.ModulePath), BindingFlags.Public | BindingFlags.Static)!
                    .GetValue(null);

                if (modulePath is not null)
                {
                    modulePath = new Uri(new Uri(NavigationManager.BaseUri), modulePath).AbsoluteUri;
                }

                await RegisterCustomElementWithJavaScriptAsync(registration.Key, localName, modulePath);
            }
        }
    }

    private async Task RegisterCustomElementWithJavaScriptAsync(string identifier, string? localName = null, string? modulePath = null)
        => await JSRuntime.InvokeVoidAsync("window.blazorWebComponents.registerCustomElement", identifier, localName, modulePath);
}
