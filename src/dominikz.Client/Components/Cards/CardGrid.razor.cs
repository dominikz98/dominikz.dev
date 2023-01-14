using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components.Cards;

public partial class CardGrid
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
