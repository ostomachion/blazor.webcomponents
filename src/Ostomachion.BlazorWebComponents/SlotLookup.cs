using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Ostomachion.BlazorWebComponents;

/// <summary>
/// A helper class to lookup the <c>*Slot</c> property of a property marked with a <see cref="SlotAttribute"/>.
/// </summary>
public class SlotLookup
{
    private readonly ImmutableDictionary<string, RenderFragment> _lookup;

    /// <summary>
    /// Initializes a new instance of <see cref="SlotLookup"/>.
    /// </summary>
    /// <param name="lookup">The lookup values.</param>
    public SlotLookup(ImmutableDictionary<string, RenderFragment> lookup)
    {
        _lookup = lookup;
    }

    /// <summary>
    /// Gets a <see cref="RenderFragment"/> representing the specified property marked with a <see cref="SlotAttribute"/>.
    /// </summary>
    /// <param name="property">A property marked with a <see cref="SlotAttribute"/>.</param>
    /// <param name="propertyName">The name of the property corresponding to the slot to retrieve.</param>
    /// <returns>A <see cref="RenderFragment"/> representing the specified property marked with a <see cref="SlotAttribute"/></returns>
    /// <exception cref="ArgumentException">if the provided property is not marked with <see cref="SlotAttribute"/>.</exception>
    public RenderFragment this[object? property, [CallerArgumentExpression(nameof(property))] string propertyName = default!]
        => _lookup.TryGetValue(propertyName, out var fragment) ? fragment : throw new ArgumentException($"'{propertyName}' is not a slot property.");
}
