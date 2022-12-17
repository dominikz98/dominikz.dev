using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Components.Chips;

public partial class ChipSelection<T>
{
    public const int ExpandLimiter = 5;

    [Parameter]
    public string? Title { get; set; }

    private List<T> _values = new();

    [Parameter]
    public List<T> Values
    {
        get => AllowExpand && !IsExpanded ? _values.Take(ExpandLimiter).ToList() : _values;
        set => _values = value;
    }

    [Parameter]
    public Func<T, string> TextFormatter { get; set; } = (x) => x?.ToString() ?? string.Empty;

    [Parameter]
    public bool IsExpanded { get; set; }

    [Parameter]
    public bool AllowExpand { get; set; }

    [Parameter]
    public bool AllowMultiSelect { get; set; }

    [Parameter]
    public T? Init { get; set; }

    [Parameter]
    public List<T> Selected { get; set; } = new();

    [Parameter]
    public EventCallback<List<T>> SelectedChanged { get; set; }

    protected List<Chip<T>> Refs = new();
    protected Chip<T>? ChipRef { set => Refs.Add(value!); }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        if (Init is null)
            return;

        await SelectInitChip();
    }

    private async Task<bool> SelectInitChip()
    {
        var chip = Refs.Where(x => x.Value != null)
            .Where(x => x.Value!.Equals(Init))
            .FirstOrDefault();

        if (chip is null)
            return false;

        chip.Select();
        await CallOnChanged(chip);
        return true;
    }

    private void CallOnExpand(string _)
        => IsExpanded = !IsExpanded;

    private async Task CallOnChanged(object sender)
    {
#pragma warning disable BL0005 // Component parameter should not be set outside of its component.
        if (AllowMultiSelect == false)
            foreach (var chip in Refs.Where(x => x != sender))
                chip.IsSelected = false;
#pragma warning restore BL0005 // Component parameter should not be set outside of its component.

        Selected = Refs.Where(x => x.IsSelected)
            .Where(x => x.Value != null)
            .Select(x => x.Value!)
            .ToList();

        if (Selected.Count == 0)
        {
            var success = await SelectInitChip();
            if (success)
                return;
        }

        await SelectedChanged.InvokeAsync(Selected);
    }
}
