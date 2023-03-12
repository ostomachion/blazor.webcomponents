namespace Ostomachion.Blazor.WebComponents.Generators;

internal record class SlotSourceInformation
{
    public string Name { get; set; } = default!;
    public string Namespace { get; set; } = default!;
    public SlotInformation[] Slots { get; set; } = default!;

    private SlotSourceInformation() { }

    public SlotSourceInformation(CustomElementClassInformation info)
    {
        Name = info.Name;
        Namespace = info.Namespace;
        Slots = info.Slots!;
    }

    public static IEnumerable<SlotSourceInformation> Group(IEnumerable<SlotSourceInformation> list)
        => list.GroupBy(x => (x!.Name, x!.Namespace))
            .Select(g => new SlotSourceInformation
            {
                Name = g.Key.Name,
                Namespace = g.Key.Namespace,
                Slots = g.SelectMany(x => x!.Slots).ToArray()
            });
}