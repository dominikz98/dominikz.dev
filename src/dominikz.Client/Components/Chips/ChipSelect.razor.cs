using dominikz.Client.Utils;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components.Chips;

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

    [Parameter] public Func<T, string> TextFormatter { get; set; } = EnumFormatter.ToString;
    [Parameter] public bool IsExpanded { get; set; }
    [Parameter] public bool AllowExpand { get; set; }
    [Parameter] public bool AllowSelect { get; set; } = true;
    [Parameter] public bool AllowMultiSelect { get; set; }
    [Parameter] public List<T> Selected { get; set; } = new();
    [Parameter] public EventCallback<T> ChipClicked { get; set; }
    [Parameter] public EventCallback<List<T>> SelectedChanged { get; set; }

    public void Select(T? value)
    {
        if (AllowSelect == false || value is null)
            return;
        
        Select(new List<T>() { value.Value });
    }
    public void Select(List<T> values)
    {
        if (AllowSelect == false)
            return;
        
        if (AllowMultiSelect == false)
        {
            Selected.Clear();
            Selected = values.GetRange(0, 1);
        }
        else
            Selected = values;

        // Check if expand is required
        if (IsExpanded
            || !AllowExpand
            || Values.Intersect(values).Count() != values.Count) 
            return;
        
        CallOnExpand();
        StateHasChanged();
    }

    private void CallOnExpand()
        => IsExpanded = !IsExpanded;

    private async Task OnDeselectClicked(T value)
    {
        if (AllowSelect)
        {
            Selected.Remove(value);
            Values.Add(value);
        }

        await ChipClicked.InvokeAsync(value);
        await SelectedChanged.InvokeAsync(Selected);
    }

    private async Task OnSelectClicked(T value)
    {
        if (AllowMultiSelect == false)
            Selected.Clear();

        if (AllowSelect)
        {
            Selected.Add(value);
            Values.Remove(value);            
        }
        
        await ChipClicked.InvokeAsync(value);
        await SelectedChanged.InvokeAsync(Selected);
    }
}