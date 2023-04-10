using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components.Picker;

public partial class FloatingNumberPicker
{
    [Parameter] public decimal Value { get; set; }
    [Parameter] public EventCallback<decimal> ValueChanged { get; set; }
    [Parameter] public decimal Min { get; set; }
    [Parameter] public decimal Max { get; set; } = decimal.MaxValue;

    private async Task CallValueChanged(ChangeEventArgs? args)
    {
        if (decimal.TryParse(args?.Value?.ToString(), out var value))
            Value = value;
        
        await ValueChanged.InvokeAsync(Value);   
    }
}