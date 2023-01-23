using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components;

public partial class Slider
{
    [Parameter] public int Min { get; set; } = 1;
    [Parameter] public int Max { get; set; } = 100;
    [Parameter] public int Step { get; set; } = 1;
    [Parameter] public int Value { get; set; }
    [Parameter] public EventCallback<int> ValueChanged { get; set; }
    
    private async Task CallValueChanged(ChangeEventArgs? args)
    {
        if (int.TryParse(args?.Value?.ToString(), out var value))
            Value = value;
        
        await ValueChanged.InvokeAsync(Value);   
    }
}