using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Components.Chips;

public partial class Chip<T>
{
    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public T? Value { get; set; }

    [Parameter]
    public bool AllowSelect { get; set; } = true;

    [Parameter]
    public EventCallback<object> OnChanged { get; set; }

    [Parameter]
    public bool IsSelected { get; set; }

    private async void CallOnChange()
    {
        IsSelected = !IsSelected;
        await OnChanged.InvokeAsync(this);
    }

    public bool Select()
        => IsSelected = true;
}
