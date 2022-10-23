using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Components.Buttons;

public partial class IconButton
{
    public Guid Id { get; } = Guid.NewGuid();

    [Parameter]
    public string? Icon { get; set; }

    [Parameter]
    public string? Text { get; set; }

    [Parameter]
    public bool IsSelected { get; set; }

    [Parameter]
    public bool AllowSelection { get; set; }

    [Parameter]
    public EventCallback<Guid> OnClick { get; set; }

    private async Task CallOnClick()
    {
        await OnClick.InvokeAsync(Id);

        if (AllowSelection == false)
            return;

        IsSelected = !IsSelected;
    }
}
