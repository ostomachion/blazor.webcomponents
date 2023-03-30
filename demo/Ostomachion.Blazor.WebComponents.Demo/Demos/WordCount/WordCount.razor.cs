using Timer = System.Timers.Timer;

namespace Ostomachion.Blazor.WebComponents.Demo.Demos.WordCount;

[CustomElement("word-count", Extends = "p")]
public partial class WordCount : WebComponentBase, IDisposable
{
    private string _text = "Loading...";

    private readonly Timer _timer = new(200);

    protected override void OnInitialized()
    {
        _timer.Elapsed += (s, e) => TimerElapsed();
        _timer.Start();
    }

    public void TimerElapsed()
    {
        _ = InvokeAsync(async () =>
        {
            var count = await InvokeJSAsync<int>("countWords");
            var text = $"Words: {count}";
            if (_text != text)
            {
                _text = text;
                StateHasChanged();
            }
        });
    }

    public void Dispose() => _timer.Dispose();
}
