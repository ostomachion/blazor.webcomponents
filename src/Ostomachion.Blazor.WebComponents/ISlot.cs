namespace Ostomachion.Blazor.WebComponents;

public interface ISlot
{
    string? Name { get; }
    object? RenderedValue { get; }
    string? ElementName { get; }
}