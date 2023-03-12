using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Ostomachion.BlazorWebComponents;

/// <inheritdoc cref="ICustomElementRegistrar"/>
public partial class CustomElementRegistrar : ICustomElementRegistrar
{
    private Dictionary<string, Type> _registrations { get; } = new();

    public IImmutableDictionary<string, Type> Registrations => _registrations.ToImmutableDictionary();

    /// <inheritdoc cref="ICustomElementRegistrar.Register{TComponent}(string?)"/>
    public void Register<TComponent>(string? identifier = null)
        where TComponent : CustomElementBase
        => Register(typeof(TComponent), identifier);

    /// <inheritdoc cref="ICustomElementRegistrar.Register(Type, string?)"/>
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

        ThrowIfInvalidIdentifier(identifier);

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

    /// <inheritdoc cref="ICustomElementRegistrar.RegisterAll(Assembly)"/>
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

    // https://html.spec.whatwg.org/multipage/custom-elements.html#valid-custom-element-name
    [GeneratedRegex(@"^[a-z]([-\.0-9_a-z\u00b7\u00c0-\u00d6\u00d8-\u00f6\u00f8-\u037d\u037f-\u1fff\u200c-\u200d\u203f-\u2040\u2070-\u218f\u2c00-\u2fef\u3001-\ud7ff\uf900-\ufdcf\ufdf0-\ufffd]|[\ud800-\udbff][\udc00-\udfff])*-([-\.0-9_a-z\u00b7\u00c0-\u00d6\u00d8-\u00f6\u00f8-\u037d\u037f-\u1fff\u200c-\u200d\u203f-\u2040\u2070-\u218f\u2c00-\u2fef\u3001-\ud7ff\uf900-\ufdcf\ufdf0-\ufffd]|[\ud800-\udbff][\udc00-\udfff])*\z", RegexOptions.ExplicitCapture)]
    private static partial Regex PotentialCustomElementName();
    private static readonly string[] ReservedNames = {
        "annotation-xml",
        "color-profile",
        "font-face",
        "font-face-src",
        "font-face-uri",
        "font-face-format",
        "font-face-name",
        "missing-glyph",
    };

    private static void ThrowIfInvalidIdentifier(string identifier, [CallerArgumentExpression(nameof(identifier))] string? paramName = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(identifier, paramName);

        if (!PotentialCustomElementName().IsMatch(identifier) || ReservedNames.Contains(identifier))
        {
            throw new ArgumentException($"{identifier} is not a valid custom element identifier.", paramName);
        }
    }
}
