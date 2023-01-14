using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components;

public partial class TextArea
{
    [Parameter] public string Value { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public int Rows { get; set; } = 20;

    private async Task CallOnChanged()
        => await ValueChanged.InvokeAsync(Value);
}