namespace dominikz.dev.Shared;

public partial class MainLayout
{
    public bool IsMenuOpen { get; set; }
    private string _classByUrl = string.Empty;

    private void OnExpandClicked()
        => IsMenuOpen = !IsMenuOpen;

    private void OnClassByUrlChanged(string value)
        => _classByUrl = value;
}
