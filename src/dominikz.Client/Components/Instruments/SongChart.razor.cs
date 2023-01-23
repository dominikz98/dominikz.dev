using dominikz.Domain.ViewModels.Songs;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components.Instruments;

public partial class SongChart
{
    [Parameter] public SongVm? Song { get; set; }

    private readonly List<LaneChart> _chartRefs = new();

    private LaneChart? ChartRef
    {
        set => _chartRefs.Add(value!);
    }

    protected override void OnInitialized()
    {
        if (Song is null)
            return;

        if (Song.Top.Count != Song.Bottom.Count)
            throw new ArgumentException("Top- and bottom lane count mismatch!");
    }

    public void ToggleSelect(int segmentIdx, int laneIdx, Tone tone)
        => _chartRefs.Where(x => x.SegmentIdx == segmentIdx)
            .Where(x => x.LaneIdx == laneIdx)
            .FirstOrDefault()
            ?.ToggleSelect(tone);
}