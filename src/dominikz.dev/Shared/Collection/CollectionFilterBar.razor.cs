using dominikz.dev.Utils;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Shared.Collection;
public partial class CollectionFilterBar<T>
{
    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public EventCallback<List<T>> OnChanged { get; set; }

    [Parameter]
    public List<T> Values { get; set; } = new();

    [Parameter]
    public List<T> SelectedValues { get; set; } = new();

    [Parameter]
    public Func<T, string> TextAccessor { get; set; } = (x) => x?.ToString() ?? string.Empty;

    [Parameter]
    public bool AllowExpand { get; set; } = true;

    [Parameter]
    public bool AllowMultiSelect { get; set; } = true;

    [Inject]
    protected BrowserService? Browser { get; set; }

    protected List<Chip<T>> Refs = new();
    protected Chip<T>? ChipRef { set => Refs.Add(value!); }
    protected Guid ContainerId = Guid.NewGuid();
    protected Guid WrapperId = Guid.NewGuid();

    protected bool IsExpanded;
    protected bool IsExpandable;
    protected int? ContainerWidth;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!AllowExpand || IsExpandable || IsExpanded)
            return;

        var window = await Browser!.GetWindow();
        var width = await Browser!.GetWidthById(ContainerId.ToString());
        IsExpandable = width <= 0 || window.Width < width + 110;
        if (IsExpandable == false)
            return;

        var availableWidth = await Browser!.GetWidthById(WrapperId.ToString());
        ContainerWidth = (int)availableWidth - 42;
        StateHasChanged();
    }

    private void OnExpandChanged()
    {
        IsExpanded = !IsExpanded;
    }

    private async Task CallOnChanged(object sender)
    {
        if (AllowMultiSelect == false)
            foreach (var chip in Refs.Where(x => x != sender))
                chip.DeSelect();

        var values = Refs.Where(x => x.IsSelected)
            .Where(x => x.Value != null)
            .Select(x => x.Value!)
            .ToList();

        await OnChanged.InvokeAsync(values);
    }
}
