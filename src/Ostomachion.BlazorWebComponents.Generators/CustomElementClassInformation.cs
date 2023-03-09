namespace Ostomachion.BlazorWebComponents.Generators;

internal record class CustomElementClassInformation
{
    public string OriginalFilePath { get; set; } = default!;
    public RelevantType RelevantType { get; set; }
    public string Name { get; set; } = default!;
    public string Namespace { get; set; } = default!;
    public SlotInformation[]? Slots { get; set; }
}
