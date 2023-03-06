using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Reflection;

namespace Ostomachion.BlazorWebComponents;
public static class RootComponentMappingCollectionExtensions
{
    public static void RegisterWebComponent<TComponent>(this RootComponentMappingCollection rootComponentMappings, string identifier)
        where TComponent : IComponent
        => rootComponentMappings.RegisterWebComponent(typeof(TComponent), identifier);

    private static void RegisterWebComponent(this RootComponentMappingCollection rootComponentMappings, Type type, string identifier)
    {
        var registeredIdentifiers = BlazorWebComponentManager.GetRegisteredTypes(rootComponentMappings);
        if (!registeredIdentifiers.Any())
        {
            rootComponentMappings.Add<BlazorWebComponentManager>("head::after");
        }
        else if (registeredIdentifiers.ContainsKey(identifier))
        {
            throw new InvalidOperationException($"The identifier {identifier} has already been registered.");
        }

        type.GetProperty(nameof(IWebComponent.Identifier), BindingFlags.Public | BindingFlags.Static)!
            .SetValue(null, identifier);

        BlazorWebComponentManager.RegisterComponent(rootComponentMappings, type, identifier);
    }

    public static void RegisterAllWebComponents(this RootComponentMappingCollection rootComponentMappings, Assembly assembly)
    {
        foreach (var type in assembly.DefinedTypes)
        {
            if (type.IsAssignableTo(typeof(WebComponentBase)))
            {
                if (!BlazorWebComponentManager.GetRegisteredTypes(rootComponentMappings).ContainsValue(type))
                {
                    var identifier = type.FullName?.ToLower().Replace('.', '-') ?? throw new NotSupportedException("Cannot get full name of type.");
                    rootComponentMappings.RegisterWebComponent(type, identifier);
                }
            }
        }
    }
}
