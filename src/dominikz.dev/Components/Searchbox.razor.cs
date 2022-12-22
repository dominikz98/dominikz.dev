using dominikz.dev.Utils;
using Microsoft.AspNetCore.Components;
using Timer = System.Timers.Timer;

namespace dominikz.dev.Components;

public partial class Searchbox
{
    [Parameter] public string? Value { get; set; }

    [Parameter] public EventCallback<string?> ValueChanged { get; set; }

    [Parameter] public bool DelayInputTrigger { get; set; } = true;

    [Parameter] public string? QueryBinding { get; set; }

    [Inject] protected NavigationManager? NavManager { get; set; }

    private Timer _inputTimer = new(TimeSpan.FromSeconds(0.3));

    public Searchbox()
    {
        _inputTimer.Elapsed += async (_, a) =>
        {
            _inputTimer.Stop();

            // handle query binding
            if (string.IsNullOrWhiteSpace(QueryBinding) == false)
                NavManager!.AttachOrUpdateQuery(QueryBinding, Value);

            else
                // handle data binding
                await ValueChanged.InvokeAsync(Value);
        };
    }

    protected override void OnInitialized()
    {
        if (string.IsNullOrWhiteSpace(QueryBinding) != false
            || !NavManager!.TryGetQueryByKey<string>(QueryBinding, out var search)) 
            return;
        
        Value = search;
    }

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