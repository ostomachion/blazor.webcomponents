namespace Ostomachion.Blazor.WebComponents;

/// <summary>
/// An interface that should be implemented by all classes that inherit <see cref="WebComponentBase"/>.
/// </summary>
public interface IWebComponent : ICustomElement
{
    /// <summary>
    /// The contents of the CSS stylesheet that will be attached to each rendered shadow DOM of this component.
    /// </summary>
    public static abstract string? Stylesheet { get; }
}
