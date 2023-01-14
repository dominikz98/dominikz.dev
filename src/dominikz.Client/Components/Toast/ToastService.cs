using System.Timers;
using Timer = System.Timers.Timer;

namespace dominikz.Client.Components.Toast;

public class ToastService : IDisposable
{
    public event Action<string, ToastLevel>? OnShow;
    public event Action? OnHide;
    private readonly Timer _countdown = new(2500);

    public ToastService()
    {
        _countdown.Elapsed += Hide;
        _countdown.AutoReset = false;
    }

    public void Show(string message, ToastLevel level)
    {
        OnShow?.Invoke(message, level);
        StartCountdown();
    }

    private void StartCountdown()
    {
        if (_countdown.Enabled)
        {
            _countdown.Stop();
            _countdown.Start();
            return;
        }

        _countdown.Start();
    }

    private void Hide(object? source, ElapsedEventArgs args)
        => OnHide?.Invoke();

    public void Dispose()
        => _countdown.Dispose();
}