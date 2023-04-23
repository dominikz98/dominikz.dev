using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components.Buttons;

public partial class TextButton
{
    [Parameter] public int Id { get; set; }
    [Parameter] public string? Text { get; set; }
    [Parameter] public bool AllowSelection { get; set; }
    [Parameter] public bool IsSelected { get; set; }
    [Parameter] public EventCallback<TextButton> OnClick { get; set; }
    [Parameter] public bool Disabled { get; set; }

    private async Task CallOnClick()
    {
        if (AllowSelection)
            ToggleSelect();

        await OnClick.InvokeAsync(this);
    }

    public void ToggleSelect()
        => IsSelected = !IsSelected;
}