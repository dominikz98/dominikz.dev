using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components.Cards;

public partial class HorizontalCard
{
    [Parameter] public string ImageUrl { get; set; } = string.Empty;
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public bool Highlight { get; set; }
    [Parameter] public string? RedirectUrl { get; set; } = string.Empty;
    [Parameter] public RenderFragment? ChildContent { get; set; }
}