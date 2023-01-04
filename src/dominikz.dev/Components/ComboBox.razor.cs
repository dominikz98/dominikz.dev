using dominikz.dev.Utils;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Components;

public partial class ComboBox<T> where T : struct, Enum
{
    [Parameter] public T? Selected { get; set; }
    [Parameter] public EventCallback<T> SelectedChanged { get; set; }
    [Parameter] public List<T> Values { get; set; } = new();
    [Parameter] public Func<T, string> TextAccessor { get; set; } = EnumFormatter.ToString;
    [Parameter] public string? Placeholder { get; set; }

    public async Task CallSelectedChanged(ChangeEventArgs args)
    {
        var selectedAsString = args?.Value?.ToString();
        if (Enum.TryParse<T>(selectedAsString, out var parsed) == false)
            return;

        Select(parsed);
        if (Selected != null)
            await SelectedChanged.InvokeAsync(Selected.Value);
    }

    public void Select(T value)
        => Selected = value;
}