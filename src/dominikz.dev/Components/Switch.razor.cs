
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Components;

public partial class Switch
{
    [Parameter] public int State { get; set; }
    
    [Parameter] public EventCallback<int> StateChanged { get; set; }

    private async Task OnStateChanged(ChangeEventArgs e)
    {
        SetState(e?.Value);
        await StateChanged.InvokeAsync(State);
    }

    public void SetState(object? state)
    {
        var valueRaw = state?.ToString();
        if (bool.TryParse(valueRaw, out var value) == false)
            return;
        
        State = value ? 1 : 0;
    }
}
