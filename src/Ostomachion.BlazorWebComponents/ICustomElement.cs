namespace Ostomachion.BlazorWebComponents;

/// <summary>
/// An interface that should be implemented by all classes that inherit <see cref="CustomElementBase"/>.
/// </summary>
public interface ICustomElement
{
    public static abstract string? Identifier { get; set; }
    public static abstract string LocalName { get; }
}
