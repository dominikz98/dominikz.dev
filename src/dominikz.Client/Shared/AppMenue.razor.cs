using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace dominikz.Client.Shared;

public partial class AppMenue
{
    [Parameter]
    public bool IsExpanded { get; set; }

    [Inject]
    protected NavigationManager? Navigation { get; set; }

    private static List<MenueEntry> _pages = new()
        {
            new MenueEntry("fa-rss", "Blog", "/blog"),
            new MenueEntry("fa-film", "Movies", "/movies"),
            new MenueEntry("fa-compact-disc", "Songs", "/songs"),
            new MenueEntry("fa-lemon", "Cookbook", "/cookbook")
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