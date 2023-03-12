namespace Ostomachion.Blazor.WebComponents.Generators;

internal readonly record struct NameInfo()
{
    public string Name { get; } = default!;
    public string Namespace { get; } = default!;
    public string? LocalName { get; }

    public NameInfo(string name, string namespaceName, string? localName)
        : this()
    {
        Name = name;
        Namespace = namespaceName;
        LocalName = localName;
    }

    public static IEnumerable<NameInfo> Group(IEnumerable<NameInfo> list)
        => list.GroupBy(x => (x!.Name, x!.Namespace))
            .Select(g => new NameInfo
            (
                g.Key.Name,
                g.Key.Namespace,
                g.FirstOrDefault(x => x!.LocalName is not null).LocalName
            ));
}
