using dominikz.Client.Utils;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components.Theme;

public partial class ThemeProvider
{
    [Parameter]
    public Theme Theme { get; set; } = Theme.Dark;

    [Inject]
    protected BrowserService? Browser { get; set; }

    [Inject]
    protected NavigationManager? Navigation { get; set; }
}
