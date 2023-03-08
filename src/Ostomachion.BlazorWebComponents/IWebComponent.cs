namespace Ostomachion.BlazorWebComponents;

public interface IWebComponent : ICustomElement
{
    public static abstract string? Stylesheet { get; }
}
