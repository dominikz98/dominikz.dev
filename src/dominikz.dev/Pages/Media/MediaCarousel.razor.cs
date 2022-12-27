using dominikz.dev.Utils;
using dominikz.shared.ViewModels;
using dominikz.shared.ViewModels.Media;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Pages.Media;

public partial class MediaCarousel
{
    [Parameter]
    public List<MediaPreviewVM> Data { get; set; } = new();

    [Inject]
    protected BrowserService? Browser { get; set; }

    private async Task OnPrevClicked()
        => await Browser!.ChangeCarouselScrollLeft(false);

    private async Task OnNextClicked()
        => await Browser!.ChangeCarouselScrollLeft(true);
}
