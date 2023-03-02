using System.Collections;

namespace Ostomachion.BlazorWebComponents;

public class ClassList : ISet<string>
{
    public event EventHandler? Updated;

    private readonly HashSet<string> _value = new();

    public int Count => _value.Count;

    public bool IsReadOnly => false;

    private static readonly char[] AsciiWhitespace = "\t\n\f\r ".ToCharArray();

    public bool Add(string className)
    {
        ArgumentException.ThrowIfNullOrEmpty(className);
        if (className.Union(AsciiWhitespace).Any())
        {
            throw new ArgumentException("The token cannot contain whitespace.", nameof(className));
        }

        var added = _value.Add(className);
        if (added)
        {
            OnUpdated(EventArgs.Empty);
        }

        return added;
    }

    public void SetFromClassString(string? classes)
    {
        _value.Clear();
        if (classes is not null)
        {
            var list = classes.Split(AsciiWhitespace, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in list)
            {
                _ = _value.Add(item);
            }
        }

        OnUpdated(EventArgs.Empty);
    }

    public void Clear()
    {
        _value.Clear();

        OnUpdated(EventArgs.Empty);
    }

    public bool Contains(string item) => _value.Contains(item);

    public void CopyTo(string[] array, int arrayIndex) => _value.CopyTo(array, arrayIndex);

    public void ExceptWith(IEnumerable<string> other) => _value.ExceptWith(other);

    public IEnumerator<string> GetEnumerator() => _value.GetEnumerator();

    public void IntersectWith(IEnumerable<string> other) => _value.IntersectWith(other);

    public bool IsProperSubsetOf(IEnumerable<string> other) => _value.IsProperSubsetOf(other);

    public bool IsProperSupersetOf(IEnumerable<string> other) => _value.IsProperSupersetOf(other);

    public bool IsSubsetOf(IEnumerable<string> other) => _value.IsSubsetOf(other);

    public bool IsSupersetOf(IEnumerable<string> other) => _value.IsSupersetOf(other);

    public bool Overlaps(IEnumerable<string> other) => _value.Overlaps(other);

    public bool Remove(string item)
    {
        var removed = _value.Remove(item);
        if (removed)
        {
            OnUpdated(EventArgs.Empty);
        }

        return removed;
    }

    public bool SetEquals(IEnumerable<string> other) => _value.SetEquals(other);

    public void SymmetricExceptWith(IEnumerable<string> other) => _value.SymmetricExceptWith(other);

    public void UnionWith(IEnumerable<string> other) => _value.UnionWith(other);

    void ICollection<string>.Add(string item) => _ = Add(item);

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_value).GetEnumerator();

    public override string? ToString() => _value.Any() ? String.Join(' ', _value) : null;

    protected virtual void OnUpdated(EventArgs e) => Updated?.Invoke(this, e);
}
