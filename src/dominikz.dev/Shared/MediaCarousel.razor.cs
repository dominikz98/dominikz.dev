using dominikz.dev.Utils;
using dominikz.kernel.ViewModels;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Shared;

public partial class MediaCarousel
{
    [Parameter]
    public List<MediaVM> Data { get; set; } = new();

    [Parameter]
    public int Height { get; set; } = 300;

    [Inject]
    protected BrowserService? Browser { get; set; }

    private async Task OnPrevClicked()
        => await Browser!.ChangeCarouselScrollLeft(false);

    private async Task OnNextClicked()
        => await Browser!.ChangeCarouselScrollLeft(true);
}
