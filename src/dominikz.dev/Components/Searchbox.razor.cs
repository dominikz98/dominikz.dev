using dominikz.dev.Utils;
using Microsoft.AspNetCore.Components;
using Timer = System.Timers.Timer;

namespace dominikz.dev.Components;

public partial class Searchbox
{
    [Parameter] public string? Value { get; set; }

    [Parameter] public EventCallback<string?> OnValueChanged { get; set; }

    [Parameter] public bool DelayInputTrigger { get; set; } = true;

    private Timer _inputTimer = new(TimeSpan.FromSeconds(0.3));

    public Searchbox()
    {
        _inputTimer.Elapsed += async (_, _) =>
        {
            _inputTimer.Stop();

            // handle data binding
            await OnValueChanged.InvokeAsync(Value);
        };
    }

    public void SetValue(string? value)
        => Value = value;
    
    private async Task CallOnChanged()
    {
        if (DelayInputTrigger == false)
        {
            await OnValueChanged.InvokeAsync(Value);
            return;
        }

        _inputTimer.Stop();
        _inputTimer.Start();
    }
}