using dominikz.dev.Utils;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Shared.Tables;

public partial class Table<T>
{
    [Parameter]
    public IReadOnlyCollection<T> Values { get; set; } = new List<T>();

    [Parameter]
    public IReadOnlyCollection<ColumnDefinition<T>> Columns { get; set; } = new List<ColumnDefinition<T>>();

    [Parameter]
    public bool ShowIndex { get; set; }

    [Parameter]
    public Action<T> OnRowClicked { get; set; } = (x) => { };

    [Inject]
    protected BrowserService? Browser { get; set; }

    private bool _isMobile;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        _isMobile = (await Browser!.GetWindow()).IsMobile;
        StateHasChanged();
    }

    private string? GetSum(ColumnDefinition<T> column)
    {
        if (column.Actions.HasFlag(ColumnActionFlags.SUM) == false)
            return null;

        var sum = Values.Select(x => column.Accessor(x))
            .Select(x => x?.ToString())
            .Where(x => double.TryParse(x, out _))
            .Select(x => double.Parse(x!))
            .Sum();

        return column.Formatter(sum);
    }
}

public class ColumnDefinition<T>
{
    public string? Name { get; private set; }
    public Func<T, object?> Accessor { get; private set; }
    public Func<object?, string> Formatter { get; set; } = (x) => x?.ToString() ?? string.Empty;

    public ColumnActionFlags Actions { get; set; }

    public ColumnDefinition(string name, Func<T, object?> accessor)
    {
        Name = name;
        Accessor = accessor;
    }
}

[Flags]
public enum ColumnActionFlags
{
    NONE = 0,
    SUM = 1,
    LINK = 2,
    HIDE_ON_MOBILE = 4
}