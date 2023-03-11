using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Ostomachion.BlazorWebComponents;

/// <summary>
/// Represents the classes of an HTML element.
/// </summary>
public class ClassList : ISet<string>
{
    private static readonly char[] AsciiWhitespace = "\t\n\f\r ".ToCharArray();

    private readonly HashSet<string> _value = new();

    /// <summary>
    /// Gets the number of class names on the element.
    /// </summary>
    public int Count => _value.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <summary>
    /// An <see cref="Action"/> which is invoked when a class is added or removed from the <see cref="ClassList"/>.
    /// </summary>
    public Action OnUpdated { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeSet"/> class.
    /// </summary>
    public ClassList()
    {
        OnUpdated = () => { };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeSet"/> class.
    /// </summary>
    /// <param name="onUpdated">
    /// An <see cref="Action"/> which will be invoked when a class is added or removed from the <see cref="ClassList"/>.
    /// </param>
    public ClassList(Action onUpdated)
    {
        OnUpdated = onUpdated;
    }

    /// <summary>
    /// Adds a class to the <see cref="ClassList"/> and returns a value to indicate if the class was successfully added.
    /// </summary>
    /// <param name="className"></param>
    /// <returns>
    /// <see langword="true"/> if the class is added to the <see cref="ClassList"/>;
    /// <see langword="false"/> class is already in the <see cref="ClassList"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">If <paramref name="className"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="className"/> is not a valid class name.</exception>
    public bool Add(string className)
    {
        ThrowIfInvalidName(className);
        var added = _value.Add(className);
        if (added)
        {
            OnUpdated();
        }

        return added;
    }

    /// <summary>
    /// Clears and sets the <see cref="ClassList"/> using the value of a <c>class</c> attribute.
    /// if <paramref name="classes"/> is <see langword="null"/>, the list is cleared.
    /// </summary>
    /// <param name="classes">A list of class names separated by ASCII-whitespace characters, or <see langword="null"/>.</param>
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

        OnUpdated();
    }

    /// <summary>
    /// Removes all classes from the <see cref="ClassList"/> object.
    /// </summary>
    public void Clear()
    {
        _value.Clear();

        OnUpdated();
    }

    /// <summary>
    /// <code>Determines whether the <see cref="ClassList"/> contains a specified class.</code>
    /// </summary>
    /// <param name="className">The class to locate in the <see cref="ClassList"/>.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="className"/> is found in the <see cref="ClassList"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">If <paramref name="className"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="className"/> is not a valid class name.</exception>
    public bool Contains(string className)
    {
        ThrowIfInvalidName(className);
        return _value.Contains(className);
    }

    /// <inheritdoc/>
    public void CopyTo(string[] array, int arrayIndex) => _value.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<string> other) => _value.ExceptWith(other);

    /// <inheritdoc/>
    public IEnumerator<string> GetEnumerator() => _value.GetEnumerator();

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<string> other) => _value.IntersectWith(other);

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<string> other) => _value.IsProperSubsetOf(other);

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<string> other) => _value.IsProperSupersetOf(other);

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<string> other) => _value.IsSubsetOf(other);

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<string> other) => _value.IsSupersetOf(other);

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<string> other) => _value.Overlaps(other);

    /// <summary>
    /// Removes the class from the <see cref="ClassList"/>.
    /// </summary>
    /// <param name="className">
    /// <see langword="true" if <paramref name="className"/> was successfully removed from the <see cref="ClassList"/>;
    /// otherwise, <see langword="false"/>. This method also returns <see langword="false"/> if <paramref name="className"/>
    /// is not found in the <see cref="ClassList"/>.
    /// </param>
    /// <returns>The class to remove from the <see cref="ClassList"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="className"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="className"/> is not a valid class name.</exception>
    public bool Remove(string className)
    {
        ThrowIfInvalidName(className);
        var removed = _value.Remove(className);
        if (removed)
        {
            OnUpdated();
        }

        return removed;
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<string> other) => _value.SetEquals(other);

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<string> other) => _value.SymmetricExceptWith(other);

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<string> other) => _value.UnionWith(other);

    /// <inheritdoc/>
    void ICollection<string>.Add(string item) => _ = Add(item);

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_value).GetEnumerator();

    /// <inheritdoc/>
    public override string? ToString() => _value.Any() ? String.Join(' ', _value) : null;

    private static void ThrowIfInvalidName([NotNull] string name, [CallerArgumentExpression(nameof(name))] string? paramName = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(name, paramName);

        if (name.Union(AsciiWhitespace).Any())
        {
            throw new ArgumentException("The token cannot contain whitespace.", nameof(name));
        }
    }
}
