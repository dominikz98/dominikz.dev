using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Components;

public partial class Searchbox
{
    [Parameter]
    public string? Value { get; set; }

    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    public async Task CallOnChanged()
        => await ValueChanged.InvokeAsync(Value);
}
