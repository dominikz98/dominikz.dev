using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Components.Instruments;

public partial class PianoKey
{
    [Parameter]
    public Tone? Tone { get; set; }

    [Parameter]
    public EventCallback<Tone> OnClicked { get; set; }

    public bool IsSelected { get; private set; }

    private string _cssClasses = string.Empty;

    protected override void OnInitialized()
        => OverrideStyle();

    public async Task CallOnClicked()
    {
        if (Tone is null)
            return;

        await OnClicked.InvokeAsync(Tone.Value);
    }

    public void Select()
    {
        IsSelected = true;
        OverrideStyle();
    }

    public void DeSelect()
    {
        IsSelected = false;
        OverrideStyle();
    }

    private void OverrideStyle()
    {
        var classes = new List<string>();

        if (Tone?.IsGroupTone ?? false)
            classes.Add("piano-key-black");
        else
            classes.Add("piano-key");

        if (IsSelected)
            classes.Add("piano-key-selected");

        _cssClasses = string.Join(" ", classes);
        StateHasChanged();
    }
}