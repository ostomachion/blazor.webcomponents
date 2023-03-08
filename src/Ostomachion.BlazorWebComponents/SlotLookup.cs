using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Ostomachion.BlazorWebComponents;
public class SlotLookup
{
    private readonly ImmutableDictionary<string, RenderFragment> _lookup;

    public SlotLookup(ImmutableDictionary<string, RenderFragment> lookup)
    {
        _lookup = lookup;
    }

    public RenderFragment this[object? property, [CallerArgumentExpression(nameof(property))] string propertyName = default!]
        => _lookup.TryGetValue(propertyName, out var fragment) ? fragment : throw new ArgumentException($"'{propertyName}' is not a slot property.");
}
