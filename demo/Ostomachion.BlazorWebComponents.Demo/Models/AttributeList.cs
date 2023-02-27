using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace Ostomachion.BlazorWebComponents.Demo.Models;

public class AttributeList : IDictionary<string, RenderFragment>
{
    private readonly AttributeListKeyedCollection _value = new();

    public RenderFragment this[string key]
    {
        get => _value[key].Value;
        set
        {
            _ = _value.Remove(key);
            _value.Add(new(key, value));
        }
    }

    public ICollection<string> Keys => _value.Select(x => x.Key).ToList();

    public ICollection<RenderFragment> Values => _value.Select(x => x.Value).ToList();

    public int Count => _value.Count;

    public bool IsReadOnly => false;

    public void Add(string key, RenderFragment value) => _value.Add(new(key, value));
    public void Add(KeyValuePair<string, RenderFragment> item) => _value.Add(item);
    public void Clear() => _value.Clear();
    public bool Contains(KeyValuePair<string, RenderFragment> item) => _value.Contains(item);
    public bool ContainsKey(string key) => _value.Contains(key);
    public void CopyTo(KeyValuePair<string, RenderFragment>[] array, int arrayIndex) => _value.CopyTo(array, arrayIndex);
    public IEnumerator<KeyValuePair<string, RenderFragment>> GetEnumerator() => _value.GetEnumerator();
    public bool Remove(string key) => _value.Remove(key);
    public bool Remove(KeyValuePair<string, RenderFragment> item) => _value.Remove(item);
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out RenderFragment value)
    {
        var success = _value.TryGetValue(key, out var pair);
        value = success ? pair.Value : null;
        return success;
    }

    IEnumerator IEnumerable.GetEnumerator() => _value.GetEnumerator();

    private class AttributeListKeyedCollection : KeyedCollection<string, KeyValuePair<string, RenderFragment>>
    {
        protected override string GetKeyForItem(KeyValuePair<string, RenderFragment> item) => item.Key;
    }
}