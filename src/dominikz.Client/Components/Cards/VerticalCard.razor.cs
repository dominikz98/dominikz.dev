﻿using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components.Cards;

public partial class VerticalCard
{
    [Parameter]
    public string ImageUrl { get; set; } = string.Empty;

    [Parameter]
    public string Title { get; set; } = string.Empty;

    [Parameter]
    public string? CoverText { get; set; }

    [Parameter]
    public EventCallback OnClick { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
