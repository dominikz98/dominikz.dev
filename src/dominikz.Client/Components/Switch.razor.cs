
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components;

public partial class Switch
{
    [Parameter] public bool State { get; set; }
    
    [Parameter] public EventCallback<bool> StateChanged { get; set; }

    private async Task CallStateChanged(ChangeEventArgs? args)
    {
        var valueRaw = args?.Value?.ToString();
        var value = bool.TryParse(valueRaw, out var parsed) && parsed;
        await StateChanged.InvokeAsync(value);
    }
}
