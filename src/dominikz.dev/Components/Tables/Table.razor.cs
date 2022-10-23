using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Components.Tables;

public partial class Table<T>
{
    [Parameter]
    public List<T> Values { get; set; } = new List<T>();

    [Parameter]
    public IReadOnlyCollection<ColumnDefinition<T>> Columns { get; set; } = new List<ColumnDefinition<T>>();

    [Parameter]
    public bool ShowIndex { get; set; }

    [Parameter]
    public Action<T> OnRowClicked { get; set; } = (x) => { };

    private string? GetColCssClass(ColumnDefinition<T> column)
    {
        if (column.Actions.HasFlag(ColumnActionFlags.HIDE_ON_MOBILE))
            return "table-col-hide-on-mobile";

        return null;
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
