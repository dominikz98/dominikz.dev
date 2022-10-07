using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Shared;

public partial class Searchbox
{
    private string? _value;

    [Parameter]
    public EventCallback<string?> OnChanged { get; set; }

    public async Task CallOnChanged()
        => await OnChanged.InvokeAsync(_value);
}
