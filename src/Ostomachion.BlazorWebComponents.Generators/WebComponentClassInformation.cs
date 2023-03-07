namespace Ostomachion.BlazorWebComponents.Generators;

internal record class WebComponentClassInformation
{
    public string FilePath { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Namespace { get; set; } = default!;
    public SlotInformation[] Slots { get; set; } = default!;
}
