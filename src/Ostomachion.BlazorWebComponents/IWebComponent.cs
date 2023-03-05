namespace Ostomachion.BlazorWebComponents;

public interface IWebComponent
{
    public static abstract string? Identifier { get; set; }

    public static abstract string? StylesheetUrl { get; }
}
