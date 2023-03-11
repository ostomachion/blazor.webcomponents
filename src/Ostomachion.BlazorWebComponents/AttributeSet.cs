using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Ostomachion.BlazorWebComponents;

/// <summary>
/// Represents the attributes on an HTML element.
/// </summary>
public class AttributeSet : IDictionary<string, object?>
{
    private readonly Dictionary<string, object> _value = new();

    /// <summary>
    /// Represents the value of the "class" attribute.
    /// </summary>
    public ClassList ClassList { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeSet"/> class.
    /// </summary>
    public AttributeSet()
    {
        ClassList = new(ClassListUpdated);
    }

    /// <summary>
    /// Gets or sets the attribute with the specified name.
    /// </summary>
    /// <param name="name">The name of the attribute to get or set.</param>
    /// <returns>The attribute with the specified name.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="name"/> is not a valid attribute name.</exception>
    public object? this[string name]
    {
        get
        {
            ThrowIfInvalidName(name);
            return _value.TryGetValue(name, out var value) ? value : null;
        }
        set
        {
            ThrowIfInvalidName(name);
            if (name == "class")
            {
                ClassList.SetFromClassString(value?.ToString());
            }
            else if (value is null)
            {
                _ = _value.Remove(name);
            }
            else if (_value.ContainsKey(name))
            {
                _value[name] = value;
            }
            else
            {
                _value.Add(name, value);
            }
        }
    }

    private void ClassListUpdated()
    {
        var classList = ClassList.ToString();
        if (classList is null)
        {
            _ = _value.Remove("class");
        }
        else if (_value.ContainsKey("class"))
        {
            _value["class"] = classList;
        }
        else
        {
            _value.Add("class", classList);
        }
    }

    /// <inheritdoc />
    public ICollection<string> Keys => _value.Keys;

    /// <inheritdoc />
    public ICollection<object?> Values => _value.Values!;

    /// <inheritdoc />
    public int Count => _value.Count;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <summary>
    /// Adds, updates, or removes an attribute with the provided name and value to the <see cref="AttributeSet"/>.
    /// </summary>
    /// <param name="name">The name of the element to add.</param>
    /// <param name="value">The object to use as the value of the attribute to add.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="name"/> is not a valid attribute name.</exception>
    public void Add(string name, object? value)
    {
        ThrowIfInvalidName(name);
        this[name] = value;
    }

    /// <summary>
    /// Adds, updates, or removes an attribute with the provided name and value to the <see cref="AttributeSet"/>.
    /// </summary>
    /// <param name="item">The attribute to add tot he <see cref="AttributeSet"/>.</param>
    /// <exception cref="ArgumentNullException">If the <c>Key</c> of <paramref name="item"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If the <c>Key</c> of <paramref name="item"/> is not a valid attribute name.</exception>
    public void Add(KeyValuePair<string, object?> item)
    {
        ThrowIfInvalidName(item);
        this[item.Key] = item.Value;
    }

    /// <summary>
    /// Removes all attributes.
    /// </summary>
    public void Clear() => _value.Clear();

    /// <summary>
    /// Determines whether the <see cref="AttributeSet"/> contains a specified attribute.
    /// </summary>
    /// <param name="item">The attribute to located in the <see cref="AttributeSet"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="AttributeSet"/> contains an attribute with the specified
    /// name and value, or if the <c>Value</c> of <see cref="item"/> is null and the <see cref="AttributeSet"/>
    /// does not contain an attribute with the specified name; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">If the <c>Key</c> of <paramref name="item"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If the <c>Key</c> of <paramref name="item"/> is not a valid attribute name.</exception>
    public bool Contains(KeyValuePair<string, object?> item)
    {
        ThrowIfInvalidName(item);
        return item.Value is null && !_value.ContainsKey(item.Key) || _value.Contains(item!);
    }

    /// <summary>
    /// Determines whether the <see cref="AttributeSet"/> contains an attribute with the specified name.
    /// </summary>
    /// <param name="name">The name to locate in the <see cref="AttributeSet"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="AttributeSet"/> contains an attribute with the name; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="name"/> is not a valid attribute name.</exception>
    public bool ContainsKey(string name)
    {
        ThrowIfInvalidName(name);
        return _value.ContainsKey(name);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex) => _value.ToArray().CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => _value.GetEnumerator();

    /// <summary>
    /// Removes the attribute with the specified name from the <see cref="AttributeSet"/>.
    /// </summary>
    /// <param name="name">The name of the attribute to remove.</param>
    /// <returns>
    /// <see langword="true"/> if the attribute is successfully removed; otherwise, <see langword="false"/>. This method
    /// also returns <see langword="false"/> if <paramref name="name"/> was not found in the <see cref="AttributeSet"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="name"/> is not a valid attribute name.</exception>
    /// <exception cref="NotSupportedException"/>
    public bool Remove(string name)
    {
        ArgumentNullException.ThrowIfNull(nameof(name));
        return _value.Remove(name);
    }

    /// <summary>
    /// Removes the first occurrence of a specified attribute from the <see cref="AttributeSet"/>.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>
    /// <para>
    /// <see langword="true"/> if <paramref name="item"/> was successfully removed from the <see cref="AttributeSet"/>;
    /// otherwise <see langword="false"/>. This method also returns <see langword="false"/> if <paramref name="item"/>
    /// was not found in the <see cref="AttributeSet"/>.
    /// </para>
    /// <para>
    /// Note that if the <c>Value</c> of <paramref name="item"/> is <see langword="null"/>, the item is considered to be
    /// "found" if the <see cref="AttributeSet"/> <i>does not</i> contain an attribute with the specified name.
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">If the <c>Key</c> of <paramref name="item"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If the <c>Key</c> of <paramref name="item"/> is not a valid attribute name.</exception>
    public bool Remove(KeyValuePair<string, object?> item)
    {
        ThrowIfInvalidName(item);
        var found = Contains(item);
        if (found)
        {
            _ = _value.Remove(item.Key);
        }

        return found;
    }

    /// <summary>
    /// Gets the value associated with the specified name.
    /// </summary>
    /// <param name="name">The name whose value to get.</param>
    /// <param name="value">
    /// When this method returns, the value associated with the specified key, if the key is found;
    /// otherwise, <see langword="null"/>. This parameter is passed uninitialized.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="AttributeSet"/> contains an attribute with the specified name;
    /// otherwise <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <see langword="null"/>"</exception>
    /// <exception cref="ArgumentException">If <paramref name="name"/> is not a valid attribute name.</exception>
    public bool TryGetValue(string name, [MaybeNullWhen(false)] out object? value)
    {
        ThrowIfInvalidName(name);
        value = this[name];
        return value is not null;
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_value).GetEnumerator();

    private static void ThrowIfInvalidName([NotNull] string name, [CallerArgumentExpression(nameof(name))] string? paramName = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(name, paramName);

        // TODO:
    }

    private static void ThrowIfInvalidName([NotNull] KeyValuePair<string, object?> item, [CallerArgumentExpression(nameof(item))] string? paramName = null)
    {
        ArgumentNullException.ThrowIfNull(item, paramName);

        try
        {
            ThrowIfInvalidName(item.Key, paramName);
        }
        catch (ArgumentNullException ex)
        {
            throw new ArgumentException(ex.Message, paramName);
        }
    }
}
