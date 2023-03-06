using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Ostomachion.BlazorWebComponents.Demo.Models;

namespace Ostomachion.BlazorWebComponents.Demo.Demos.EditableList;

[WebComponent("editable-list")]
public partial class EditableList
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
