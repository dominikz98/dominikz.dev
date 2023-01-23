using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components.Instruments;

public partial class ToneCandle
{
    [Parameter] public Tone? Value { get; set; }
    [Parameter] public bool IsSelected { get; set; }

    public void ToggleSelect()
        => IsSelected = !IsSelected;
}