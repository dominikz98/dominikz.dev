using dominikz.Domain.ViewModels.Songs;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components.Instruments;

public partial class SongPreview
{
    [Parameter] public SongVm? Song { get; set; }

    private int _topGap;
    private int _bottomGap;

    protected override void OnInitialized()
    {
        _topGap = GetMinGap(Song?.Top.FirstOrDefault());
        _bottomGap = GetMinGap(Song?.Bottom.FirstOrDefault());
    }

    private int GetMinGap(LaneVm? lane)
    {
        var positions = lane?
                            .Notes
                            .Select(x => x.Position)
                            .OrderBy(x => x)
                            .ToList()
                        ?? new List<int>();

        return positions.Skip(1)
            .Select((x, y) => x - positions[y])
            .Min();
    }

    private bool IsJumpAvailable(bool top, int position)
        => top
            ? IsJumpAvailable(Song?.Top.FirstOrDefault(), position, _topGap)
            : IsJumpAvailable(Song?.Bottom.FirstOrDefault(), position, _bottomGap);

    private bool IsJumpAvailable(LaneVm? lane, int position, int gap)
    {
        if (lane is null)
            return false;

        var ticks = Enumerable.Range(position + 1, Math.Min(gap - 1, lane.AvailableTicks - position));
        var positions = lane?
                            .Notes
                            .Select(x => x.Position)
                            .OrderBy(x => x)
                            .ToList()
                        ?? new List<int>();

        return ticks.Intersect(positions).Any() == false;
    }

    private NoteVm? GetNote(bool top, int position)
        => top
            ? Song?.Top.FirstOrDefault()?.Notes.FirstOrDefault(x => x.Position == position)
            : Song?.Bottom.FirstOrDefault()?.Notes.FirstOrDefault(x => x.Position == position);
}