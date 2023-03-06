namespace Ostomachion.BlazorWebComponents;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class WebComponentAttribute : Attribute
{
    public string? DefaultName { get; }

    public WebComponentAttribute(string? defaultName = null)
    {
        DefaultName = defaultName;
    }
}