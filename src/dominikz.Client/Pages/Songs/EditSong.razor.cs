using dominikz.Domain.Enums.Music;
using dominikz.Domain.ViewModels.Songs;

namespace dominikz.Client.Pages.Songs;

public partial class EditSong
{
    private LaneVm _topLane = new()
    {
        Clef = ClefEnum.Treble,
        Tact = TactEnum.T4Of4,
        AvailableTicks = (int)TactEnum.T4Of4 * (int)NoteTypeEnum.ThirtySecond,
        Notes = new List<NoteVm>()
    };

    private LaneVm _bottomLane = new()
    {
        Clef = ClefEnum.Bass,
        Tact = TactEnum.T4Of4,
        AvailableTicks = (int)TactEnum.T4Of4 * (int)NoteTypeEnum.ThirtySecond,
        Notes = new List<NoteVm>()
    };
}