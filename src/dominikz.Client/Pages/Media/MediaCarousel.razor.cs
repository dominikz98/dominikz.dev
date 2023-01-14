using dominikz.Client.Utils;
using dominikz.Domain.ViewModels.Media;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Pages.Media;

public partial class MediaCarousel
{
    [Parameter]
    public List<MediaPreviewVm> Data { get; set; } = new();

    [Inject]
    protected BrowserService? Browser { get; set; }

    private async Task OnPrevClicked()
        => await Browser!.ChangeCarouselScrollLeft(false);

    private async Task OnNextClicked()
        => await Browser!.ChangeCarouselScrollLeft(true);
}
