namespace Ostomachion.BlazorWebComponents;

public interface ICustomElement
{
    public static abstract string? Identifier { get; set; }
    public static abstract string LocalName { get; }
}
