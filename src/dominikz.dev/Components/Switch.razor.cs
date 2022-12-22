
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Components;

public partial class Switch
{
    [Parameter] public int State { get; set; }
    
    [Parameter] public EventCallback<int> StateChanged { get; set; }

    private async Task OnStateChanged(ChangeEventArgs e)
    {
        var valueRaw = e?.Value?.ToString();
        if (bool.TryParse(valueRaw, out var value) == false)
            return;

        State = value ? 1 : 0;
        await StateChanged.InvokeAsync(State);
    }
}
