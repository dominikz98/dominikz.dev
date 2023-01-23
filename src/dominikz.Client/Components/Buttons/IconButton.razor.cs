using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components.Buttons;

public partial class IconButton
{
    private Guid _id = Guid.NewGuid();

    [Parameter] public string? Icon { get; set; }

    [Parameter] public string? Text { get; set; }

    [Parameter] public bool IsSelected { get; set; }

    [Parameter] public bool IsRounded { get; set; }

    [Parameter] public bool AllowSelection { get; set; }

    [Parameter] public EventCallback<Guid> OnClick { get; set; }

    [Parameter] public bool Disabled { get; set; }

    private async Task CallOnClick()
    {
        if (Disabled)
            return;

        await OnClick.InvokeAsync(_id);

        if (AllowSelection == false)
            return;

        IsSelected = !IsSelected;
    }

    private string ConcatClasses()
    {
        var classes = new List<string>();

        if (IsSelected)
            classes.Add("icon-button-selected");

        if (IsRounded)
            classes.Add("icon-button-rounded");

        if (Disabled)
            classes.Add("icon-button-disabled");

        return string.Join(" ", classes);
    }
}