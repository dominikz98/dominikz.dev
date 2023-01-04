using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Components.Chips;

public partial class Chip<T> where T : struct
{
    [Parameter] public string? Title { get; set; }

    [Parameter] public T? Value { get; set; }

    [Parameter] public bool AllowSelect { get; set; } = true;

    [Parameter] public EventCallback<T?> Clicked { get; set; }

    [Parameter] public bool IsSelected { get; set; }

    private async void CallOnChange()
    {
        T? value;
        if (AllowSelect)
        {
            IsSelected = !IsSelected;
            value = IsSelected ? Value : null;
        }
        else
            value = Value;
         
        await Clicked.InvokeAsync(value);
    }

    public void ToggleSelect()
        => IsSelected = !IsSelected;
}