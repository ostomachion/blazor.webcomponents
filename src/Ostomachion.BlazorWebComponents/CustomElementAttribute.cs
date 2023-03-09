namespace Ostomachion.BlazorWebComponents;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class CustomElementAttribute : Attribute
{
    public string? DefaultIdentifier { get; }
    public string? Extends { get; set; }

    public CustomElementAttribute(string? defaultIdentifier = null)
    {
        DefaultIdentifier = defaultIdentifier;
    }
}