using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Shared;

public partial class Chip<T>
{
    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public T? Value { get; set; }

    [Parameter]
    public EventCallback<object> OnChanged { get; set; }

    public bool IsSelected { get; set; }

    public void DeSelect()
    {
        IsSelected = false;
    }

    private async void CallOnChange()
    {
        IsSelected = !IsSelected;
        await OnChanged.InvokeAsync(this);
    }
}
