namespace Ostomachion.BlazorWebComponents;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class SlotAttribute : Attribute
{
    public string SlotName { get; }

    public bool IsTemplated { get; set; }

    public string? RootElement { get; set; }

    public SlotAttribute(string slotName)
    {
        SlotName = slotName;
    }
}
