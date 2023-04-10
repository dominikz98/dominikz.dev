using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components.Picker;

public partial class DatePicker
{
    [Parameter] public DateTime? Date { get; set; }
    [Parameter] public EventCallback<DateTime?> DateChanged { get; set; }
    [Parameter] public bool Disabled { get; set; }
    
    private async Task CallDateChanged(ChangeEventArgs? args)
    {
        var valueAsString = args?.Value?.ToString();
        var date = DateTime.TryParse(valueAsString, out var parsed) ? parsed : (DateTime?)null;
        await DateChanged.InvokeAsync(date);
    }
}