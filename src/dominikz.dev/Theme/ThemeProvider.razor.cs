using dominikz.dev.Utils;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Theme;

public partial class ThemeProvider
{
    [Parameter]
    public Theme Theme { get; set; } = Theme.Dark;

    [Parameter]
    public EventCallback<string> ClassByUrlChanged { get; set; }

    [Inject]
    protected BrowserService? Browser { get; set; }

    [Inject]
    protected NavigationManager? Navigation { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Navigation!.LocationChanged += async (sender, args) => await LocationChanged(sender, args.Location);
        await LocationChanged(this, Navigation.Uri);
    }

    private async Task LocationChanged(object? _, string location)
    {
        // Invoke changed event
        var uri = new Uri(location);
        var classByUrl = Theme.ClassesByUrls.FirstOrDefault(x => uri.AbsolutePath.StartsWith(x.Key));
        if (string.IsNullOrWhiteSpace(classByUrl.Key))
            classByUrl = Theme.ClassesByUrls.First();

        await ClassByUrlChanged.InvokeAsync(classByUrl.Value);
    }
}
