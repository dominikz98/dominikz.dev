using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace dominikz.dev.Components.Chips;

public partial class EditableChip
{
    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string> ValueChanged { get; set; }

    private bool _editMode;
    private string? _original;
    
    private void OnClick()
    {
        _original = Value;
        _editMode = !_editMode;
    }

    private async Task OnLostFocus()
    {
        Value = string.IsNullOrWhiteSpace(Value) ? null : Value;
        _editMode = false;

        var changed = Value != _original;
        if (changed)
            await ValueChanged.InvokeAsync(Value);
    }

    public void Clear()
    {
        _editMode = false;
        Value = null;
    }
}