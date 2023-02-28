using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Ostomachion.BlazorWebComponents;
public class AttributeSet : IDictionary<string, object?>
{
    private readonly Dictionary<string, object> _value = new();

    public object? this[string key]
    {
        get => _value.TryGetValue(key, out var value) ? value : null;
        set
        {
            if (value is null)
            {
                _ = _value.Remove(key);
            }
            else if (_value.ContainsKey(key))
            {
                _value[key] = value;
            }
            else
            {
                _value.Add(key, value);
            }
        }
    }

    public ICollection<string> Keys => _value.Keys;

    public ICollection<object?> Values => _value.Values!;

    public int Count => _value.Count;

    public bool IsReadOnly => false;

    public void Add(string key, object? value) => this[key] = value;

    public void Add(KeyValuePair<string, object?> item) => this[item.Key] = item.Value;

    public void Clear() => _value.Clear();

    public bool Contains(KeyValuePair<string, object?> item)
    {
        return item.Value is null && !_value.ContainsKey(item.Key) || _value.Contains(item!);
    }

    public bool ContainsKey(string key) => _value.ContainsKey(key);

    public void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex) => _value.ToArray().CopyTo(array, arrayIndex);

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => _value.GetEnumerator();

    public bool Remove(string key) => _value.Remove(key);

    public bool Remove(KeyValuePair<string, object?> item)
    {
        if (Contains(item))
        {
            _ = _value.Remove(item.Key);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out object? value)
    {
        value = this[key];
        return value is not null;
    }

    IEnumerator IEnumerable.GetEnumerator() => _value.GetEnumerator();
}
