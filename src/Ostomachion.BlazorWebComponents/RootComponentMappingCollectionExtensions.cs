using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Reflection;

namespace Ostomachion.BlazorWebComponents;
public static class RootComponentMappingCollectionExtensions
{
    public static void RegisterCustomElement<TComponent>(this RootComponentMappingCollection rootComponentMappings, string? identifier = null)
        where TComponent : CustomElementBase
        => rootComponentMappings.RegisterCustomElement(typeof(TComponent), identifier);

    private static void RegisterCustomElement(this RootComponentMappingCollection rootComponentMappings, Type type, string? identifier = null)
    {
        if (!type.IsAssignableTo(typeof(CustomElementBase)))
        {
            throw new ArgumentException($"Unable to register the component {type.FullName} because it does not inherit from ${nameof(CustomElementBase)}.");
        }

        // Try to get default name from attribute.
        identifier ??= type.GetCustomAttribute<CustomElementAttribute>()?.DefaultName;

        // If we still don't have a name, construct one from the qualified name if possible.
        identifier ??= type.FullName?.ToLower().Replace('.', '-') ?? throw new NotSupportedException($"Cannot create an identifier for the component {type.FullName}.");

        var registeredIdentifiers = BlazorWebComponentManager.GetRegisteredTypes(rootComponentMappings);
        if (!registeredIdentifiers.Any())
        {
            rootComponentMappings.Add<BlazorWebComponentManager>("head::after");
        }
        else if (registeredIdentifiers.TryGetValue(identifier, out var component))
        {
            throw new InvalidOperationException($"Unable to register the component {type.FullName} with identifier {identifier} " +
                $"because the identifier has already been registered for the {component.FullName}.");
        }

        type.GetProperty(nameof(ICustomElement.Identifier), BindingFlags.Public | BindingFlags.Static)!
            .SetValue(null, identifier);

        BlazorWebComponentManager.RegisterComponent(rootComponentMappings, type, identifier);
    }

    public static void RegisterAllCustomElements(this RootComponentMappingCollection rootComponentMappings, Assembly assembly)
    {
        var types = assembly.DefinedTypes
            .Where(t => t.IsAssignableTo(typeof(CustomElementBase)))
            .Where(t => !BlazorWebComponentManager.GetRegisteredTypes(rootComponentMappings).ContainsValue(t));

        foreach (var type in types)
        {
            rootComponentMappings.RegisterCustomElement(type);
        }
    }
}
