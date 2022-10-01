using MatBlazor;

namespace dominikz.dev.Shared;

public partial class MainLayout
{
    private NavMenu? _navMenu;
    private MatTheme _theme => new()
    {
        Background = "#242424",
        Surface = "#33383C",
        Primary = "#5f7d8f",
        OnPrimary = "#bfd319",
        OnSecondary = "#bfd319",
        OnSurface = "#5f7d8f",
        Secondary = "#bfd319",
    };

    public void ToggleSidebar()
        => _navMenu?.ToggleSidebar();
}
