using dominikz.dev.Theme;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Components;

public partial class Panel
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public CssItemAlignment ItemAlignment { get; set; } = CssItemAlignment.Center;

    [Parameter]
    public CssFlexAlignment FlexAlignment { get; set; }

    [Parameter]
    public bool UseAsSurface { get; set; }

    [Parameter]
    public bool AllowWrap { get; set; } = true;

    [Parameter]
    public int? Width { get; set; }

    [Parameter]
    public int? Gap { get; set; }

    private string CreateStyle()
    {
        var css = new ThemeClass
        {
            Width = Width,
            ItemAlignment = ItemAlignment,
            FlexAlignment = FlexAlignment,
            Gap = Gap
        };

        if (AllowWrap)
        {
            css.FlexWrap = CssFlexWrap.Wrap;
            css.Overflow = CssOverflow.Hidden;
        }

        if (UseAsSurface)
        {
            css.BorderRadius = 5;
            css.BackgroundColor = CssColor.Surface;
        }

        return css.ToString();
    }
}
