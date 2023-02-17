namespace Ostomachion.BlazorWebComponents;

public interface IWebComponent
{
    public ShadowRootMode ShadowRootMode { get; set; }

    public static abstract string TagName { get; }

    public static abstract string TemplateHtml { get; }

    public static abstract string TemplateCss { get; }
}
