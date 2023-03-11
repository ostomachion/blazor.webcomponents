using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Ostomachion.BlazorWebComponents;

/// <summary>
/// A service to track an identifier with each custom element class to be used in an app. Component classes that inherit CustomElementBase
/// (or its subclasses like WebComponentBase) must be registered before any instances are rendered. <see cref="CustomElementRegistrar"/>
/// is responsible for setting the <see cref="ICustomElement.Identifier"/> property of each <see cref="CustomElementBase"/> subclass and
/// for ensuring that the identifier is registered with the JavaScript runtime.
/// </summary>
public class CustomElementRegistrar : ICustomElementRegistrar
{
    private Dictionary<string, Type> _registrations { get; } = new();

    /// <summary>
    /// A mapping of identifiers to registered component types. Each registered type will implement <see cref="CustomElementBase"/>.
    /// </summary>
    public IImmutableDictionary<string, Type> Registrations => _registrations.ToImmutableDictionary();

    /// <summary>
    /// Registers a subclass of <see cref="CustomElementBase"/> with an identifier.
    /// </summary>
    /// <typeparam name="TComponent">The component class to register.</typeparam>
    /// <param name="identifier">The identifier to register to the component.</param>
    /// <exception cref="ArgumentException">If <paramref name="identifier"/> is not a valid identifier.</exception>
    /// <exception cref="NotSupportedException">If a valid identifier cannot be created.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="identifier"/> or <typeparamref name="TComponent"/> has already been registered.</exception>
    public void Register<TComponent>(string? identifier = null)
        where TComponent : CustomElementBase
        => Register(typeof(TComponent), identifier);

    /// <summary>
    /// Registers a subclass of <see cref="CustomElementBase"/> with an identifier.
    /// </summary>
    /// <param name="type">The component class to register. The type must inherit from <see cref="CustomElementBase"/>.</param>
    /// <param name="identifier">The identifier to register to the component.</param>
    /// <exception cref="ArgumentException">If <paramref name="identifier"/> is not a valid identifier.</exception>
    /// <exception cref="NotSupportedException">If a valid identifier cannot be created.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="identifier"/> or <typeparamref name="TComponent"/> has already been registered.</exception>
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
                $"because the identifier has already been registered for the component {existingType.FullName}.");
        }

        if (_registrations.ContainsValue(type))
        {
            throw new InvalidOperationException($"Unable to register the component {type.FullName} with identifier {identifier} " +
                $"because the component has already been registered.");
        }

        type.GetProperty(nameof(ICustomElement.Identifier), BindingFlags.Public | BindingFlags.Static)!
            .SetValue(null, identifier);

        _registrations.Add(identifier, type);
    }

    /// <summary>
    /// Finds and registers all classes in the specified assembly that inherit <see cref="CustomElementBase"/> using their
    /// default name. If a component has previously been registered, this method does not attempt to register it again.
    /// </summary>
    /// <param name="assembly">The assembly in which to look for custom element components.</param>
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

    private static void ThrowIfInvalidIdentifier(string identifier, [CallerArgumentExpression(nameof(identifier))] string? paramName = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(identifier, paramName);

        // TODO:
        if (!identifier.Contains('-'))
        {
            throw new ArgumentException($"{identifier} is not a valid custom element identifier.", paramName);
        }
    }
}
