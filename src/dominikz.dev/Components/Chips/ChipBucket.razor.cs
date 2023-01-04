using dominikz.dev.Models;
using dominikz.dev.Utils;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Components.Chips;

public partial class ChipBucket
{
    [Parameter] public string? Title { get; set; }
    [Parameter] public List<string> Values { get; set; } = new();
    [Parameter] public EventCallback<List<string>> ValuesChanged { get; set; }
    [Parameter] public List<string> Recommendations { get; set; } = new();

    private EditableChip? _addChip;
    private readonly List<EditableChip> _refs = new();

    private EditableChip? ChipRef
    {
        set => _refs.Add(value!);
    }

    private void OnAddValueChanged(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        Values.Add(value);
        _addChip?.Clear();
    }

    private async Task OnValueChanged(string original, string? current)
    {
        var index = Values.IndexOf(original);

        if (string.IsNullOrWhiteSpace(current))
            // remove
            Values.RemoveAt(index);
        else
            // update
            Values[index] = current;

        await ValuesChanged.InvokeAsync(Values);
    }

    private void OnRecommendedTagClicked(TextStruct? tag)
    {
        if (tag is null)
            return;

        if (Values.Contains(tag.Value.Text))
            return;

        var toRemove = Recommendations.Single(x => x.Equals(tag.Value.Text));
        Recommendations.Remove(toRemove);
        Values.Add(tag.Value.Text);
    }
}