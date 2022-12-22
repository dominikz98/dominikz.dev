﻿using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Components;

public partial class ComboBox
{
    [Parameter]
    public List<string> Values { get; set; } = new();

    private string _selected = string.Empty;

    [Parameter]
#pragma warning disable BL0007
    public string Selected
#pragma warning restore BL0007
    {
        get => _selected;
        set { 
            _selected = value; 
            CallOnChanged();
        }
    }

    [Parameter]
    public EventCallback<string> SelectedChange { get; set; }

    protected override void OnInitialized()
    {
        Selected = Values.FirstOrDefault() ?? string.Empty;
    }

    private async void CallOnChanged()
        => await SelectedChange.InvokeAsync(Selected);
}
