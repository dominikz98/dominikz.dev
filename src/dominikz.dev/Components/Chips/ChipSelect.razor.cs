using dominikz.dev.Utils;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Components.Chips;

public partial class ChipSelect<T> where T : struct
{
    private const int ExpandLimiter = 5;

    [Parameter] public string? Title { get; set; }

    private List<T> _values = new();

    [Parameter]
#pragma warning disable BL0007
    public List<T> Values
#pragma warning restore BL0007
    {
        get => AllowExpand && !IsExpanded ? _values.Take(ExpandLimiter).ToList() : _values;
        set => _values = value;
    }

    [Parameter] public Func<T?, string> TextFormatter { get; set; } = (value) => value == null ? string.Empty : EnumFormatter.ToString(value.Value);
    [Parameter] public bool IsExpanded { get; set; }
    [Parameter] public bool AllowExpand { get; set; }
    [Parameter] public bool AllowSelect { get; set; } = true;
    [Parameter] public T? Selected { get; set; }
    [Parameter] public EventCallback<T?> SelectedChanged { get; set; }

    private readonly List<Chip<T>> _refs = new();

    private Chip<T>? ChipRef
    {
        set => _refs.Add(value!);
    }

    public void Select(T? value)
    {
        if (AllowSelect == false)
            return;
        
        Selected = value;

        // Check if expand is required
        if (value is not null
            && IsExpanded == false
            && AllowExpand
            && Values.Contains(value.Value) == false)
        {
            CallOnExpand();
            StateHasChanged();
        }

        Refresh();
    }

    private void CallOnExpand()
        => IsExpanded = !IsExpanded;

    private async Task OnChipClicked(T? value)
    {
        var selected = _refs
            .FirstOrDefault(x => x.Value.Equals(value))
            ?.Value;

        if (AllowSelect)
        {
            Selected = selected;
            Refresh();            
        }

        await SelectedChanged.InvokeAsync(selected);
    }

    private void Refresh()
    {
        // select current value
        var toSelect = _refs.FirstOrDefault(x => x.Value.Equals(Selected));
        if (toSelect is not null && toSelect.IsSelected == false)
            toSelect.ToggleSelect();

        // deselect all
        var toDeselect = _refs.Where(x => x.Equals(toSelect) == false)
            .Where(x => x.IsSelected)
            .ToList();

        foreach (var chip in toDeselect)
            chip.ToggleSelect();
    }
}