namespace dominikz.Client.Shared;

public partial class MainLayout
{
    private bool _isMenuOpen;

    private void OnExpandClicked()
        => _isMenuOpen = !_isMenuOpen;
}
