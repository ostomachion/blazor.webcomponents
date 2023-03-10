using System.Collections.Immutable;
using System.Reflection;

namespace Ostomachion.BlazorWebComponents;
public class CustomElementRegistrar : ICustomElementRegistrar
{
    private Dictionary<string, Type> _registrations { get; } = new();

    public IImmutableDictionary<string, Type> Registrations => _registrations.ToImmutableDictionary();

    public void Register<TComponent>(string? identifier = null)
        where TComponent : CustomElementBase
        => Register(typeof(TComponent), identifier);

    public void Register(Type type, string? identifier = null)
    {
        if (!type.IsAssignableTo(typeof(CustomElementBase)))
        {
            throw new ArgumentException($"Unable to register the component {type.FullName} because it does not inherit from ${nameof(CustomElementBase)}.");
        }

        // Try to get default name from attribute.
        identifier ??= type.GetCustomAttribute<CustomElementAttribute>()?.DefaultIdentifier;

        // If we still don't have a name, construct one from the qualified name if possible.
        identifier ??= type.FullName?.ToLower().Replace('.', '-') ?? throw new NotSupportedException($"Cannot create an identifier for the component {type.FullName}.");

        if (_registrations.TryGetValue(identifier, out var existingType))
        {
            throw new InvalidOperationException($"Unable to register the component {type.FullName} with identifier {identifier} " +
                $"because the identifier has already been registered for the {existingType.FullName}.");
        }

        type.GetProperty(nameof(ICustomElement.Identifier), BindingFlags.Public | BindingFlags.Static)!
            .SetValue(null, identifier);

        _registrations.Add(identifier, type);
    }

    public void RegisterAll(Assembly assembly)
    {
        var types = assembly.DefinedTypes
            .Where(t => t.IsAssignableTo(typeof(CustomElementBase)))
            .Where(t => !_registrations.ContainsValue(t));

        foreach (var type in types)
        {
            Register(type);
        }
    }
}
