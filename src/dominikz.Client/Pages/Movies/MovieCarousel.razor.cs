using dominikz.Client.Utils;
using dominikz.Domain.ViewModels.Movies;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Pages.Movies;

public partial class MovieCarousel
{
    [Parameter]
    public List<MoviePreviewVm> Data { get; set; } = new();

    [Inject]
    protected BrowserService? Browser { get; set; }

    private async Task OnPrevClicked()
        => await Browser!.ChangeCarouselScrollLeft(false);

    private async Task OnNextClicked()
        => await Browser!.ChangeCarouselScrollLeft(true);
}
