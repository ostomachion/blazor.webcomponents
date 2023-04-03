using Microsoft.AspNetCore.Components.Web;
using System.Drawing;

namespace Ostomachion.Blazor.WebComponents.Demo.Demos.LifeCycleCallbacks;

public partial class Index
{
    private bool _renderSquare;
    private int _length;
    private Color _color;

    public void AddClick(MouseEventArgs e)
    {
        _renderSquare = true;
        _length = 100;
        _color = Color.Red;
    }

    public void UpdateClick(MouseEventArgs e)
    {
        _length = Random.Shared.Next(50, 201);
        _color = Color.FromArgb(Random.Shared.Next(256), Random.Shared.Next(256), Random.Shared.Next(256));
    }

    public void RemoveClick(MouseEventArgs e)
    {
        _renderSquare = false;
    }
}
