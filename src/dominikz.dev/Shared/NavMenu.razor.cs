using dominikz.dev.Utils;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Shared;

public partial class NavMenu
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Inject]
    protected BrowserService? Browser { get; set; }

    [Inject]
    protected NavigationManager? Navigation { get; set; }

    public NavMenuEntry? Current { get; private set; }
    public const int MenuBarWidth = 250;
    public const int IconBarWidth = 72;

    private int _currentMenuWidth;
    private string? _currentTextDisplay => !_isSidebarOpen ? "none" : "inline";
    private bool _isMobile;
    private bool _isSidebarOpen;

    public List<NavMenuEntry> Menue
    {
        get => new()
        {
            new NavMenuEntry("home", "Home", "/"),
            new NavMenuEntry("explore", "Blog", "/blog"),
            new NavMenuEntry("movie_filter", "Media", "/media"),
            new NavMenuEntry("restaurant", "Cookbook", "/cookbook"),
        };
    }

    protected override void OnInitialized()
        => Current = Menue[0];

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        _isMobile = (await Browser!.GetWindow()).IsMobile;
        CalculateMenuWidth();
    }

    private void CalculateMenuWidth()
    {
        var calculated = _isSidebarOpen ? MenuBarWidth : (_isMobile ? 0 : IconBarWidth);
        if (calculated == _currentMenuWidth)
            return;

        _currentMenuWidth = calculated;
        StateHasChanged();
    }

    private void NavigateTo(NavMenuEntry entry)
    {
        Current = entry;
        Navigation!.NavigateTo(Current.Url);
        HideSidebar();
    }

    public void ToggleSidebar()
    {
        _isSidebarOpen = !_isSidebarOpen;
        CalculateMenuWidth();
    }

    private void HideSidebar()
    {
        _isSidebarOpen = false;
        CalculateMenuWidth();
    }

    private string GetSelectedState(NavMenuEntry entry)
        => entry.Url == Current?.Url
            ? "sidebar-item-selected"
            : string.Empty;
}

public record NavMenuEntry(string Icon, string Title, string Url);