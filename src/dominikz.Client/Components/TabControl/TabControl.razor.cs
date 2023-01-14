using dominikz.Client.Components.Buttons;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components.TabControl;

public partial class TabControl
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public EventCallback<int> OnPageChanged { get; set; }

    public TabPage? ActivePage;

    private TextButton? Ref
    {
        set => _refs.Add(value!);
    }

    private List<TextButton> _refs = new();
    private readonly List<TabPage> _pages = new();

    public void ShowPage(int pageId)
    {
        ActivePage = _pages.FirstOrDefault(x => x.Id == pageId);
        Refresh();
    }
    
    internal void AddPage(TabPage tabPage)
    {
        _pages.Add(tabPage);
        if (_pages.Count == 1)
            ActivePage = tabPage;

        StateHasChanged();
    }

    private async Task CallOnPageChanged(TabPage page)
    {
        ActivePage = page;
        Refresh();
        await OnPageChanged.InvokeAsync(page.Id);
    }

    private void Refresh()
    {
        // select
        var selectedButton = _refs.FirstOrDefault(x => x.Id == ActivePage?.Id);
        if (selectedButton is not null && selectedButton.IsSelected == false)
            selectedButton.ToggleSelect();

        // deselect
        foreach (var button in _refs)
        {
            if (button == selectedButton)
                continue;

            if (button.IsSelected == false)
                continue;

            button.ToggleSelect();
        }
    }
}