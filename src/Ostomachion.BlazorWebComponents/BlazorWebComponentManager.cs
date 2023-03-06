using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Collections.Immutable;

namespace Ostomachion.BlazorWebComponents;

public class BlazorWebComponentManager : ComponentBase
{
    [Inject]
    public IJSComponentConfiguration Key { get; set; } = default!;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    private static Dictionary<IJSComponentConfiguration, Dictionary<string, Type>> _registeredComponents { get; } = new();

    public static ImmutableDictionary<string, Type> GetRegisteredTypes(RootComponentMappingCollection rootComponentMappings)
        => _registeredComponents.TryGetValue(rootComponentMappings, out var registeredComponents)
            ? registeredComponents.ToImmutableDictionary()
            : ImmutableDictionary<string, Type>.Empty;

    internal static void RegisterComponent(RootComponentMappingCollection rootComponentMappings, Type type, string identifier)
    {
        if (!_registeredComponents.TryGetValue(rootComponentMappings, out var registeredComponents))
        {
            registeredComponents = new();
            _registeredComponents.Add(rootComponentMappings, registeredComponents);
        }

        if (registeredComponents.ContainsKey(identifier))
        {
            throw new InvalidOperationException($"The identifier {identifier} has already been registered.");
        }

        if (!type.IsAssignableTo(typeof(IComponent)))
        {
            throw new ArgumentException($"Component must implement {nameof(IComponent)}.", nameof(identifier));
        }

        registeredComponents.Add(identifier, type);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && _registeredComponents.TryGetValue(Key, out var registeredComponents))
        {
            foreach (var registration in registeredComponents)
            {
                await RegisterComponentWithJavaScriptAsync(registration.Key);
            }
        }
    }

    private async Task RegisterComponentWithJavaScriptAsync(string identifier)
        => await JSRuntime.InvokeVoidAsync("window.blazorWebComponents.defineCustomElement", identifier);
}
