using System.Runtime.CompilerServices;

namespace Ostomachion.BlazorWebComponents;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class SlotAttribute : Attribute
{
    public string SlotName { get; }

    public string? RootElement { get; set; }

    public SlotAttribute([CallerMemberName]string slotName = null!)
    {
        SlotName = slotName;
    }
}
