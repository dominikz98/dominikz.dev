using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Components;

public partial class NumberPicker
{
    [Parameter] public int Value { get; set; }
    [Parameter] public EventCallback<int> ValueChanged { get; set; }
    [Parameter] public int Min { get; set; } = 0;
    [Parameter] public int Max { get; set; } = int.MaxValue;

    private async Task CallValueChanged(ChangeEventArgs? args)
    {
        if (int.TryParse(args?.Value?.ToString(), out var value))
            Value = value;
        
        await ValueChanged.InvokeAsync(Value);   
    }
}