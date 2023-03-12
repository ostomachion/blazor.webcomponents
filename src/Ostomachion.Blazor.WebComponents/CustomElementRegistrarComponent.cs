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
/// RootComponentMappingCollection, Action{CustomElementRegistrar})"/>
/// </summary>
public sealed class CustomElementRegistrarComponent : ComponentBase
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
