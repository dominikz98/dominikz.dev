namespace dominikz.dev.Theme;

public class ThemeClass
{
    public int? Width { get; set; }
    public int? BorderRadius { get; set; }
    public int? Padding { get; set; }
    public int? Gap { get; set; }
    public CssFlexAlignment? FlexAlignment { get; set; }
    public CssItemAlignment? ItemAlignment { get; set; }
    public CssFlexWrap? FlexWrap { get; set; }
    public CssOverflow? Overflow { get; set; }
    public CssColor? BackgroundColor { get; set; }

    public override string ToString()
    {
        var properties = new List<string>();

        if (Width is not null)
            properties.Add($"width:{Width}px");

        if (FlexAlignment is not null)
        {
            properties.Add($"display:flex");
            properties.Add($"flex-direction:{FlexAlignment.Value.ToString().ToLower()}");
        }

        if (ItemAlignment is not null)
            properties.Add($"align-items:{ItemAlignment.Value.ToString().ToLower()}");

        if (Overflow is not null)
            properties.Add($"overflow:{Overflow.Value.ToString().ToLower()}");

        if (BorderRadius is not null)
            properties.Add($"border-radius:{BorderRadius}px");

        if (Padding is not null)
            properties.Add($"padding:{Padding}px");

        if (Gap is not null)
            properties.Add($"gap:{Gap}px");

        if (FlexWrap is not null)
        {
            var value = FlexWrap.Value switch
            {
                CssFlexWrap.Wrap => "wrap",
                _ => string.Empty
            };
            properties.Add($"flex-wrap:{value}");
        }

        if (BackgroundColor is not null)
        {
            var value = BackgroundColor.Value switch
            {
                CssColor.Surface => "var(--theme-surface)",
                _ => string.Empty
            };
            properties.Add($"background-color:{value}");
        }

        return string.Join(';', properties);
    }
}

public enum CssColor
{
    Surface
}

public enum CssFlexAlignment
{
    Row,
    Column
}

public enum CssItemAlignment
{
    Start,
    Center
}

public enum CssFlexWrap
{
    Wrap
}

public enum CssOverflow
{
    Hidden
}
