using dominikz.dev.Utils;
using dominikz.kernel.ViewModels;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Shared;

public partial class BlogArticleCard
{
    [Parameter]
    public ArticleListVM? Data { get; set; }

    [Parameter]
    public int Width { get; set; } = 300;

    [Parameter]
    public EventCallback OnClick { get; set; }

    [Inject]
    protected BrowserService? Browser { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        var window = await Browser!.GetWindow();
        if (!window.IsMobile)
            return;

        Width = window.Width - 36;
        StateHasChanged();
    }
}
