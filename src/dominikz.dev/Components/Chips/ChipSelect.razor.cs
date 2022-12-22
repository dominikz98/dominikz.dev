using dominikz.dev.Utils;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Components.Chips;

public partial class ChipSelect<T> where T : struct, Enum
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

    [Parameter] public Func<T, string> TextFormatter { get; set; } = EnumConverter.ToString;

    [Parameter] public bool IsExpanded { get; set; }

    [Parameter] public bool AllowExpand { get; set; }

    [Parameter] public T? Selected { get; set; }

    [Parameter] public EventCallback<T?> SelectedChanged { get; set; }

    [Parameter] public string? QueryBinding { get; set; }

    [Inject] protected NavigationManager? NavManager { get; set; }

    private readonly List<Chip<T>> _refs = new();

    private Chip<T>? ChipRef
    {
        set => _refs.Add(value!);
    }

    protected override void OnInitialized()
    {
        NavManager!.LocationChanged += (_, _) => Refresh();
        
        if (string.IsNullOrWhiteSpace(QueryBinding))
            return;
        
        if (NavManager.TryGetQueryByKey<T>(QueryBinding, out var selected))
            Selected = selected;
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender == false)
            return;
        
        Refresh();
    }

    private void CallOnExpand()
        => IsExpanded = !IsExpanded;

    private async Task CallOnChanged(object sender)
    {
        Selected = _refs
            .Where(x => x == sender)
            .FirstOrDefault(x => x.IsSelected)
            ?.Value;

        // un-/select chips
        Refresh();
        
        // handle query binding
        if (string.IsNullOrWhiteSpace(QueryBinding) == false)
        {
            NavManager!.AttachOrUpdateQuery(QueryBinding, Selected?.ToString());
            return;
        }

        // handle data binding
        await SelectedChanged.InvokeAsync(Selected); ;
    }

    private void Refresh()
    {
        // deselect all
        foreach (var chip in _refs)
            if (chip.IsSelected)
                chip.ToggleSelect();

        if (Selected is null)
            return;

        // select current value
        foreach (var chip in _refs)
        {
            if (chip.Value.Equals(Selected.Value) == false)
                continue;
            
            chip.ToggleSelect();
        }
    }
}