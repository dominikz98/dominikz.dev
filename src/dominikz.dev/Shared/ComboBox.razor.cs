using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Shared;

public partial class ComboBox
{
    [Parameter]
    public List<string> Values { get; set; } = new();

    [Parameter]
    public EventCallback<string> OnChange { get; set; }

    private string _selected = string.Empty;
    public string Selected
    {
        get { return _selected; }
        set
        {
            _selected = value;
            CallOnChange();
        }
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender && !string.IsNullOrWhiteSpace(Selected))
            return;

        _selected = Values.FirstOrDefault() ?? string.Empty;
        StateHasChanged();
    }

    private async void CallOnChange()
        => await OnChange.InvokeAsync(Selected);
}
