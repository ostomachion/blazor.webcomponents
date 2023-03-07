namespace Ostomachion.BlazorWebComponents.Generators;

internal readonly record struct NameInfo()
{
    public string Name { get; } = default!;
    public string Namespace { get; } = default!;

    public NameInfo(string name, string namespaceName)
        : this()
    {
        Name = name;
        Namespace = namespaceName;
    }
}
