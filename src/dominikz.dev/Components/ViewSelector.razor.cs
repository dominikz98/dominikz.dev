using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Components;

public partial class ViewSelector
{
    [Parameter]
    public CollectionView Selected { get; set; }

    [Parameter]
    public EventCallback<CollectionView> SelectedChanged { get; set; }

    private async Task CallOnChanged(Guid _)
    {
        Selected = Selected == CollectionView.Grid
            ? CollectionView.Table
            : CollectionView.Grid;

        await SelectedChanged.InvokeAsync(Selected);
    }
}

public enum CollectionView
{
    Grid,
    Table
}
