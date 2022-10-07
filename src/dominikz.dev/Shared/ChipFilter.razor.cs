using dominikz.dev.Utils;
using MatBlazor;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Shared;

public partial class ChipFilter<TEnum> where TEnum : struct, Enum
{
    [Parameter]
    public List<TEnum> Values { get; set; } = Enum.GetValues<TEnum>().ToList();

    [Parameter]
    public EventCallback<TEnum> OnChanged { get; set; }

    private List<MatChip> _refs = new();
    private MatChip? _chipRef { set => _refs.Add(value!); }
    private Dictionary<string, TEnum> _mappings { get => Values.ToDictionary(x => EnumConverter.ToString(x), x => x); }
    private MatChip? _selected;

    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender)
            return;

        _selected = _refs.FirstOrDefault();
    }

    private async Task CallOnChanged()
    {
        if (_selected is null)
            return;

        var value = _mappings[_selected.Label];
        await OnChanged.InvokeAsync(value);
    }
}
