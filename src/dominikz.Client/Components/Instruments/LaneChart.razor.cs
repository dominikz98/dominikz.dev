using dominikz.Domain.ViewModels.Songs;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components.Instruments;

public partial class LaneChart
{
    [Parameter] public LaneVm? Value { get; set; }
    [Parameter] public int LaneIdx { get; set; }
    [Parameter] public int SegmentIdx { get; set; }
    
    private int _gapSize;
    private List<ToneCandle> _candleRefs = new();

    protected ToneCandle? CandleRef
    {
        set => _candleRefs.Add(value!);
    }

    protected override void OnInitialized()
    {
        if (Value is null)
            return;

        var positions = Value
            .Notes
            .Select(x => x.Position)
            .OrderBy(x => x)
            .Distinct()
            .ToList();

        _gapSize = positions.Skip(1)
            .Select((x, y) => x - positions[y])
            .Min();
    }

    public void ToggleSelect(Tone tone)
        => _candleRefs.FirstOrDefault(x => x.Value == tone)?.ToggleSelect();

    private bool IsJumpAvailable(int position)
    {
        if (Value is null || _gapSize <= 1)
            return false;

        var ticks = Enumerable.Range(position + 1, Math.Min(_gapSize - 1, Value.AvailableTicks - position)).ToList();
        var positions = Value?
                            .Notes
                            .Select(x => x.Position)
                            .OrderBy(x => x)
                            .ToList()
                        ?? new List<int>();
        
        return ticks.Intersect(positions).Any() == false;
    }
}