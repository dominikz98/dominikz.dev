using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Components;

public partial class OrderSelector
{
    [Parameter]
    public OrderInfo Value { get; set; } = new(string.Empty, OrderDirection.Ascending);

    [Parameter]
    public EventCallback<OrderInfo> ValueChanged { get; set; }

    [Parameter]
    public List<string> OrderKeys { get; set; } = new();

    protected string Icon 
        => Value.Direction == OrderDirection.Descending 
            ? "fa-solid fa-arrow-up" 
            : "fa-solid fa-arrow-down";

    private async void CallOnDirectionChanged(Guid _)
    {
        if (Value.Direction == OrderDirection.Ascending)
            Value.Direction = OrderDirection.Descending;
        else
            Value.Direction = OrderDirection.Ascending;

        await ValueChanged.InvokeAsync(Value);
    }

    private async Task CallOnKeyChanged(string value)
    {
        Value.Key = value;
        await ValueChanged.InvokeAsync(Value);
    }
}

public class OrderInfo
{
    public string Key { get; set; }
    public OrderDirection Direction { get; set; }

    public OrderInfo(string key, OrderDirection direction)
    {
        Key = key;
        Direction = direction;
    }
}

public enum OrderDirection
{
    Ascending,
    Descending
}