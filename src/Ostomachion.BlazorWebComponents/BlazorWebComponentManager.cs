﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Collections.Immutable;
using System.Reflection;

namespace Ostomachion.BlazorWebComponents;

public class BlazorWebComponentManager : ComponentBase
{
    [Inject]
    public IJSComponentConfiguration Key { get; set; } = default!;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    private static Dictionary<IJSComponentConfiguration, Dictionary<string, Type>> _registeredComponents { get; } = new();

    public static ISet<string> GetRegisteredIdentifiers(RootComponentMappingCollection rootComponentMappings)
    {
        if (_registeredComponents.TryGetValue(rootComponentMappings, out var registeredComponents))
        {
            return registeredComponents.Keys.ToImmutableHashSet();
        }
        else
        {
            return ImmutableHashSet<string>.Empty;
        }
    }

    public static void RegisterComponent<TComponent>(RootComponentMappingCollection rootComponentMappings, string identifier)
        where TComponent : IComponent
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

        registeredComponents.Add(identifier, typeof(TComponent));
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
