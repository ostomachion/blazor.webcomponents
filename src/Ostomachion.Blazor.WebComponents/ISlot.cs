namespace Ostomachion.Blazor.WebComponents;

/// <summary>
/// Represents a value that can be rendered as a slot.
/// </summary>
public interface ISlot
{
    /// <summary>
    /// The name of the slot.
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// The slotted value to be rendered in the light DOM.
    /// </summary>
    object? RenderedValue { get; }

    /// <summary>
    /// The name of the containing element to add a <c>slot</c> attribute to.
    /// Note that this is not used if <see cref="Name"/> is <see langword="null"/>.
    /// </summary>
    string? ElementName { get; }
}