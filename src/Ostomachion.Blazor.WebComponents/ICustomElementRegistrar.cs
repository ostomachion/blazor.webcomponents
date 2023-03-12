using System.Collections.Immutable;
using System.Reflection;

namespace Ostomachion.Blazor.WebComponents;

/// <summary>
/// A service to track an identifier with each custom element class to be used in an app. Component classes that inherit CustomElementBase
/// (or its subclasses like WebComponentBase) must be registered before any instances are rendered. <see cref="CustomElementRegistrar"/>
/// is responsible for setting the <see cref="ICustomElement.Identifier"/> property of each <see cref="CustomElementBase"/> subclass and
/// for ensuring that the identifier is registered with the JavaScript runtime.
/// </summary>
public interface ICustomElementRegistrar
{
    /// <summary>
    /// A mapping of identifiers to registered component types. Each registered type will implement <see cref="CustomElementBase"/>.
    /// </summary>
    IImmutableDictionary<string, Type> Registrations { get; }

    /// <summary>
    /// Registers a subclass of <see cref="CustomElementBase"/> with an identifier.
    /// </summary>
    /// <typeparam name="TComponent">The component class to register.</typeparam>
    /// <param name="identifier">The identifier to register to the component.</param>
    /// <exception cref="ArgumentException">If <paramref name="identifier"/> is not a valid identifier.</exception>
    /// <exception cref="NotSupportedException">If a valid identifier cannot be created.</exception>
    /// <exception cref="InvalidOperationException">
    /// If <paramref name="identifier"/> or <typeparamref name="TComponent"/> has already been registered.
    /// </exception>
    void Register<TComponent>(string? identifier = null) where TComponent : CustomElementBase;

    /// <summary>
    /// Registers a subclass of <see cref="CustomElementBase"/> with an identifier.
    /// </summary>
    /// <param name="type">The component class to register. The type must inherit from <see cref="CustomElementBase"/>.</param>
    /// <param name="identifier">The identifier to register to the component.</param>
    /// <exception cref="ArgumentException">If <paramref name="identifier"/> is not a valid identifier.</exception>
    /// <exception cref="NotSupportedException">If a valid identifier cannot be created.</exception>
    /// <exception cref="InvalidOperationException">
    /// If <paramref name="identifier"/> or <typeparamref name="TComponent"/> has already been registered.
    /// </exception>
    void Register(Type type, string? identifier);

    /// <summary>
    /// Finds and registers all classes in the specified assembly that inherit <see cref="CustomElementBase"/> using their
    /// default name. If a component has previously been registered, this method does not attempt to register it again.
    /// </summary>
    /// <param name="assembly">The assembly in which to look for custom element components.</param>
    void RegisterAll(Assembly assembly);
}
