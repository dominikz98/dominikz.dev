using dominikz.dev.Theme;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Components.Buttons;

public partial class TextButton
{
    [Parameter] public int Id { get; set; }
    [Parameter] public string? Text { get; set; }
    [Parameter] public bool AllowSelection { get; set; }
    [Parameter] public bool IsSelected { get; set; }
    [Parameter] public CssColor Color { get; set; } = CssColor.Surface;
    [Parameter] public EventCallback<TextButton> OnClick { get; set; }

    private async Task CallOnClick()
    {
        if (AllowSelection)
            ToggleSelect();

        await OnClick.InvokeAsync(this);
    }

    public void ToggleSelect()
        => IsSelected = !IsSelected;
}