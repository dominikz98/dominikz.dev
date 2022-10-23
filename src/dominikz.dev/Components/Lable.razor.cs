using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Components;

public partial class Lable
{
    [Parameter]
    public string? Icon { get; set; }

    [Parameter]
    public object? Text { get; set; }
}
