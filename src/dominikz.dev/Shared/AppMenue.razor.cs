using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace dominikz.dev.Shared;

public partial class AppMenue
{
    [Parameter]
    public bool IsExpanded { get; set; }

    [Inject]
    protected NavigationManager? Navigation { get; set; }

    private static List<MenueEntry> _pages = new()
        {
            new MenueEntry("fa-rss", "Blog", "/blog"),
            new MenueEntry("fa-film", "Media", "/media"),
        };

    protected override void OnInitialized()
        => Navigation!.LocationChanged += LocationChanged;

    private void LocationChanged(object? sender, LocationChangedEventArgs e)
    {
        // Change expand state
        IsExpanded = false;
        StateHasChanged();
    }
}

public record MenueEntry(string Icon, string Title, string Url);