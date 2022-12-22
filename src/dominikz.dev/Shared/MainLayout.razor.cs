namespace dominikz.dev.Shared;

public partial class MainLayout
{
    private bool _isMenuOpen;
    private string _classByUrl = string.Empty;

    private void OnExpandClicked()
        => _isMenuOpen = !_isMenuOpen;

    private void OnClassByUrlChanged(string value)
        => _classByUrl = value;
}
