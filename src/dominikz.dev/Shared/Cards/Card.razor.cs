using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Shared.Cards;

public partial class Card
{
    [Parameter]
    public int Width { get; set; }

    [Parameter]
    public string ImageUrl { get; set; } = string.Empty;

    [Parameter]
    public string Title { get; set; } = string.Empty;

    [Parameter]
    public EventCallback OnClick { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public bool Highlight { get; set; }
}
