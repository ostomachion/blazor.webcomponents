using Microsoft.AspNetCore.Components;
using System.Runtime.CompilerServices;

namespace Ostomachion.Blazor.WebComponents;

/// <summary>
/// Marks a property as a template slot. The framework will create a <c>*Slot</c> property with a type of
/// <see cref="RenderFragment"/>. Its contents will be rendered in the light DOM.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class SlotAttribute : Attribute
{
    /// <summary>
    /// The identifier for the slot.
    /// </summary>
    public string SlotName { get; }

    /// <summary>
    /// Gets or sets the tag name of the element to contain the slot contents in the light DOM.
    /// </summary>
    public string? RootElement { get; set; }

    /// <summary>
    /// If <see langword="true"/>, the framework will produce a <c>*Template</c> property with a type of
    /// <see cref="RenderFragment{TValue}"/> that will be used when the property is rendered as a slot.
    /// </summary>
    public bool IsTemplated { get; set; }

    /// <summary>
    /// The text to display if the slot is rendered but the associated property is <see langword="null".
    /// Defaults to <see cref="SlotName"/>.
    /// </summary>
    public string? DefaultText { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="SlotAttribute"/>.
    /// </summary>
    /// <param name="slotName">The identifier for the slot.</param>
    public SlotAttribute([CallerMemberName] string slotName = null!)
    {
        SlotName = slotName;
    }
}
