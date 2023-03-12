namespace Ostomachion.Blazor.WebComponents.Generators;

internal record class CustomElementClassInformation
{
    public string OriginalFilePath { get; set; } = default!;
    public RelevantType RelevantType { get; set; }
    public string Name { get; set; } = default!;
    public string Namespace { get; set; } = default!;
    public string? LocalName { get; set; }
    public SlotInformation[]? Slots { get; set; }
}
