using dominikz.dev.Utils;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Shared;

public partial class Article
{
    [Parameter]
    public string? Image { get; set; }

    [Parameter]
    public int Width { get; set; } = 300;

    [Parameter]
    public string? AuthorImage { get; set; }

    [Parameter]
    public string? Author { get; set; }

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public DateTime? Date { get; set; }

    [Parameter]
    public string? Category { get; set; }

    [Parameter]
    public bool IsHighlighted { get; set; }

    [Inject]
    protected BrowserService? Browser { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        var window = await Browser!.GetWindow();
        if (!window.IsMobile)
            return;

        Width = window.Width - 50;
        StateHasChanged();
    }
}
