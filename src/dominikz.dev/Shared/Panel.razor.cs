using dominikz.dev.Utils;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Shared;

public partial class Panel
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public CSSItemAlignment ItemAlignment { get; set; }

    [Parameter]
    public bool UseAsSurface { get; set; }

    [Parameter]
    public CSSAlignment Alignment { get; set; } = CSSAlignment.Row;

    private string? _width;

    [Parameter]
    public int Width { set => _width = $"{value}px"; }

    [Parameter]
    public int Padding { get; set; } = 8;
}

public enum CSSAlignment
{
    Row,
    Column
}

public enum CSSItemAlignment
{
    Normal,
    Center
}