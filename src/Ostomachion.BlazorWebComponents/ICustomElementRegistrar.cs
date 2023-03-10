using System.Collections.Immutable;
using System.Reflection;

namespace Ostomachion.BlazorWebComponents;

public interface ICustomElementRegistrar
{
    IImmutableDictionary<string, Type> Registrations { get; }

    void Register<TComponent>(string? identifier = null) where TComponent : CustomElementBase;
    void Register(Type type, string? identifier);
    void RegisterAll(Assembly assembly);
}
