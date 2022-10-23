using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Components.Cards;

public partial class CardGrid
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
