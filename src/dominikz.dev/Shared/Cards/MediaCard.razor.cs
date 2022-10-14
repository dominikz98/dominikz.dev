using dominikz.kernel.ViewModels;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Shared.Cards;

public partial class MediaCard
{
    [Parameter]
    public int Height { get; set; }

    [Parameter]
    public int Width { get; set; }

    [Parameter]
    public bool ShowCategory { get; set; }

    [Parameter]
    public MediaVM? Data { get; set; }

    [Parameter]
    public EventCallback OnClick { get; set; }
}
