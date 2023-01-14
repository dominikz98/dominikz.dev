using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components;

public partial class TimePicker
{
    [Parameter] public TimeSpan Time { get; set; }
    [Parameter] public EventCallback<TimeSpan> TimeChanged { get; set; }
    [Parameter] public bool Disabled { get; set; }

    private async Task CallHourChanged(int value)
    {
        Time = new TimeSpan(Time.Days, value, Time.Minutes, Time.Seconds);
        await TimeChanged.InvokeAsync(Time);
    }

    private async Task CallMinutesChanged(int value)
    {
        Time = new TimeSpan(Time.Days, Time.Hours, value, Time.Seconds);
        await TimeChanged.InvokeAsync(Time);
    }
}