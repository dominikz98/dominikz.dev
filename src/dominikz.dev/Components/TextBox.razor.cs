using dominikz.dev.Utils;
using Microsoft.AspNetCore.Components;
using Timer = System.Timers.Timer;

namespace dominikz.dev.Components;

public partial class TextBox
{
    [Parameter] public string? Icon { get; set; }
    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }
    [Parameter] public string Placeholder { get; set; } = string.Empty;
    [Parameter] public bool DelayInputTrigger { get; set; }
    [Parameter] public bool IsPassword { get; set; }

    private readonly Timer _inputTimer = new(TimeSpan.FromSeconds(0.3));

    public TextBox()
    {
        _inputTimer.Elapsed += async (_, _) =>
        {
            _inputTimer.Stop();

            // handle data binding
            await ValueChanged.InvokeAsync(Value);
        };
    }

    public void SetValue(string? value)
        => Value = value;
    
    private async Task CallOnChanged()
    {
        if (DelayInputTrigger == false)
        {
            await ValueChanged.InvokeAsync(Value);
            return;
        }

        _inputTimer.Stop();
        _inputTimer.Start();
    }
}