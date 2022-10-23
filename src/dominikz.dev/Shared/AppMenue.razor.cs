using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace dominikz.dev.Shared;

public partial class AppMenue
{
    [Parameter]
    public bool IsExpanded { get; set; }

    [Inject]
    protected NavigationManager? Navigation { get; set; }

    private static List<MenueEntry> _pages => new()
        {
            new MenueEntry("fa-compass", "Blog", "/blog"),
            new MenueEntry("fa-film", "Media", "/media"),
            new MenueEntry("fa-pizza-slice", "Cookbook", "/cookbook"),
        };

    protected override void OnInitialized()
        => Navigation!.LocationChanged += (sender, args) => LocationChanged(sender, args);

    private void LocationChanged(object? sender, LocationChangedEventArgs e)
    {
        // Change expand state
        IsExpanded = false;
        StateHasChanged();
    }
}

public record MenueEntry(string Icon, string Title, string Url);