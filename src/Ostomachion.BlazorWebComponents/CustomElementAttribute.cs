namespace Ostomachion.BlazorWebComponents;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class CustomElementAttribute : Attribute
{
    public string? DefaultName { get; }

    public CustomElementAttribute(string? defaultName = null)
    {
        DefaultName = defaultName;
    }
}