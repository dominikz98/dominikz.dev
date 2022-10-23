using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Shared;

public partial class AppBar
{
    [Parameter]
    public EventCallback OnExpandClicked { get; set; }

    private async Task CallOnExpandClicked()
        => await OnExpandClicked.InvokeAsync();
}
