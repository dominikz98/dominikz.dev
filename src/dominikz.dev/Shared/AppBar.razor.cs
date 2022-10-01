using dominikz.dev.Utils;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Shared;

public partial class AppBar
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public EventCallback OnSidebarToggled { get; set; }

    private async Task ToggleSidebar()
        => await OnSidebarToggled.InvokeAsync();
}