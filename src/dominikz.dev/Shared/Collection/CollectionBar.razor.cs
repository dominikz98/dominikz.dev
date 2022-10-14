using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Shared.Collection;

public partial class CollectionBar
{
    [Parameter]
    public EventCallback<string> OnSearchChanged { get; set; }

    [Parameter]
    public EventCallback<CollectionView> OnViewChanged { get; set; }

    [Parameter]
    public EventCallback<OrderInfo> OnOrderChanged { get; set; }

    [Parameter]
    public List<string> OrderKeys { get; set; } = new();

    private IconButton? _gridBtn;
    private IconButton? _tableBtn;

    private OrderDirection _orderDirection;
    private string _orderKey = string.Empty;

    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender)
            return;

        _gridBtn!.Select();
    }

    private async void CallOnViewChanged(Guid id)
    {
        var view = CollectionView.Grid;

        if (_gridBtn!.Id == id)
            _tableBtn!.DeSelect();
        else
        {
            _gridBtn!.DeSelect();
            view = CollectionView.Table;
        }

        await OnViewChanged.InvokeAsync(view);
    }

    private async void CallOnOrderDirectionChanged(Guid _)
    {
        if (_orderDirection == OrderDirection.Ascending)
            _orderDirection = OrderDirection.Descending;
        else
            _orderDirection = OrderDirection.Ascending;

        await OnOrderChanged.InvokeAsync(new OrderInfo(_orderKey, _orderDirection));
    }

    private async void CallOnOrderKeyChanged(string key)
    {
        _orderKey = key;
        await OnOrderChanged.InvokeAsync(new OrderInfo(_orderKey, _orderDirection));
    }
}

public enum CollectionView
{
    Grid,
    Table
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