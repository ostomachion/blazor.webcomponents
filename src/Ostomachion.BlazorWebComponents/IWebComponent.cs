namespace Ostomachion.BlazorWebComponents;

public interface IWebComponent
{
    public static abstract string TagName { get; }

    public static abstract string? TemplateCss { get; }
}
