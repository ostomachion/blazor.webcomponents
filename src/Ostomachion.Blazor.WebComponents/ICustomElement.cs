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
}
