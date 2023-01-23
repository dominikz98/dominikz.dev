using dominikz.Domain.ViewModels.Songs;

namespace dominikz.Client.Components.Instruments;

public class NoteArgs : LaneArgs
{
    public NoteVm Note { get; }

    public NoteArgs(int posIndex, int laneIndex, int segmentIndex, NoteVm note) : base(posIndex, laneIndex, segmentIndex)
    {
        Note = note;
    }
}