using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Reflection;

namespace Ostomachion.BlazorWebComponents;
public static class RootComponentMappingCollectionExtensions
{
    public static void RegisterWebComponent<TComponent>(this RootComponentMappingCollection rootComponentMappings, string identifier)
        where TComponent : IComponent
    {
        var registeredIdentifiers = BlazorWebComponentManager.GetRegisteredIdentifiers(rootComponentMappings);
        if (!registeredIdentifiers.Any())
        {
            rootComponentMappings.Add<BlazorWebComponentManager>("head::after");
        }
        else if (registeredIdentifiers.Contains(identifier))
        {
            throw new InvalidOperationException($"The identifier {identifier} has already been registered.");
        }

        typeof(TComponent).GetProperty(nameof(IWebComponent.Identifier), BindingFlags.Public | BindingFlags.Static)!
            .SetValue(null, identifier);

        var namespaceParts = typeof(TComponent).Namespace!.Split('.');

        BlazorWebComponentManager.RegisterComponent<TComponent>(rootComponentMappings, identifier);
    }
}
