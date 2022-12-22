using dominikz.shared.Contracts;

namespace dominikz.shared.ViewModels;

public class SongVm
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int BPM { get; set; }
    public List<LaneVm> Top { get; set; } = new();
    public List<LaneVm> Bottom { get; set; } = new();
}

public class LaneVm
{
    public int SegmentIndex { get; set; }
    public int TickDurationInMs { get; set; }
    public int AvailableTicks { get; set; }
    public ClefEnum Clef { get; set; }
    public TactEnum Tact { get; set; }
    public List<NoteVm> Notes { get; set; } = new List<NoteVm>();
}

public class NoteVm
{
    public int Position { get; set; }
    public int Ticks { get; set; }
    public NoteTypeEnum Type { get; set; }
    public NoteEnum Note { get; set; }
    public int Segment { get; set; }
}