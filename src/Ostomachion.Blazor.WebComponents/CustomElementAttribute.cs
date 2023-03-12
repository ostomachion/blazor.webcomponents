namespace Ostomachion.Blazor.WebComponents;

/// <summary>
/// Provides extra information about the rendered custom element tag of a  <see cref="CustomElementBase"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class CustomElementAttribute : Attribute
{
    /// <summary>
    /// The identifier to use when the <see cref="CustomElementBase"/> is registered by a <see cref="CustomElementRegistrar"/>
    /// if no other name is specified. If this is <see langword="null"/> and no other name is specified, the element's identifier
    /// will be created based on the full type name of the class being registered.
    /// </summary>
    public string? DefaultIdentifier { get; }

    /// <summary>
    /// <see langword="null"/> if this is an autonomous custom element; otherwise, the name of the built-in element to customize.
    /// </summary>
    public string? Extends { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomElementAttribute"/> class.
    /// </summary>
    /// <param name="defaultIdentifier">
    /// The identifier to use when the <see cref="CustomElementBase"/> is registered by a <see cref="CustomElementRegistrar"/>
    /// if no other name is specified. If this is <see langword="null"/> and no other name is specified, the element's identifier
    /// will be created based on the full type name of the class being registered.
    /// </param>
    public CustomElementAttribute(string? defaultIdentifier = null)
    {
        DefaultIdentifier = defaultIdentifier;
    }
}
