namespace Ostomachion.Blazor.WebComponents;

/// <summary>
/// An interface that should be implemented by all classes that inherit <see cref="CustomElementBase"/>.
/// </summary>
public interface ICustomElement
{
    /// <summary>
    /// Gets or sets the identifier for the custom element.
    /// </summary>
    public static abstract string? Identifier { get; set; }

    /// <summary>
    /// <see langword="null"/> if the implementing class represents an autonomous custom element;
    /// the name of the element being extended if the implementing class represents a customized
    /// built-in element
    /// </summary>
    public static abstract string? LocalName { get; }

    /// <summary>
    /// The relative path to the JavaScript file collocated with this component, or <see langword="null"/>
    /// if no such file exists.
    /// </summary>
    public static abstract string? ModulePath { get; }
}
