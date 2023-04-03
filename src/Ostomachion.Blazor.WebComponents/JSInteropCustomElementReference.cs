namespace Ostomachion.Blazor.WebComponents;

internal class JSInteropCustomElementReference
{
    private CustomElementBaseImpl _customElement;

    public JSInteropCustomElementReference(CustomElementBaseImpl customElement)
    {
        _customElement = customElement;
    }
}
