namespace Ostomachion.Blazor.WebComponents.Demo.Models;

public class ListItem
{
    public string Value { get; set; }

    public ListItem(string value)
    {
        Value = value;
    }

    public static implicit operator ListItem(string item) => new(item);

    public override string ToString() => Value;
}
