using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Ostomachion.Blazor.WebComponents.Demo.Models;

namespace Ostomachion.Blazor.WebComponents.Demo.Demos.EditableList;

[CustomElement("editable-list")]
public partial class EditableList : WebComponentBase
{
    [Parameter]
    [EditorRequired]
    public string Title { get; set; } = default!;

    [Parameter]
    [EditorRequired]
    public List<ListItem> ListItems { get; set; } = default!;

    [Parameter]
    [EditorRequired]
    public string AddItemText { get; set; } = default!;

    public string InputValue { get; set; } = default!;

    private void RemoveListItem(ListItem listItem) => ListItems.Remove(listItem);

    private void AddListItem(MouseEventArgs e)
    {
        if (!String.IsNullOrWhiteSpace(InputValue))
        {
            ListItems.Add(InputValue);
            InputValue = String.Empty;
        }
    }
}
