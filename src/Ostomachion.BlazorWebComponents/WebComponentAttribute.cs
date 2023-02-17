namespace Ostomachion.BlazorWebComponents;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class WebComponentAttribute : Attribute
{
    public string TagName { get; }

    public WebComponentAttribute(string tagName)
    {
        TagName = tagName;
    }
}