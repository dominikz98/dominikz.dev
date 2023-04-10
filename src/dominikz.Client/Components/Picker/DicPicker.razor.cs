using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components.Picker;

public partial class DicPicker
{
    [Parameter] public Guid Selected { get; set; }
    [Parameter] public EventCallback<Guid> SelectedChanged { get; set; }
    [Parameter] public Dictionary<Guid, string> Values { get; set; } = new();

    private async Task CallSelectedChanged(ChangeEventArgs args)
    {
        if (Guid.TryParse(args.Value?.ToString(), out var id) == false)
            return;

        Selected = Values.ContainsKey(id) ? id : Guid.Empty;
        await SelectedChanged.InvokeAsync(Selected);
    }
}