using dominikz.api.Models.Structs;
using dominikz.shared.Enums;

namespace dominikz.api.Models;

public class SongSegment
{
    public int Index { get; set; }
    public Guid SongId { get; set; }

    public ClefEnum TopClef { get; set; }
    public TactEnum TopTact { get; set; }
    public NoteCollection TopNotes { get; set; }

    public ClefEnum BottomClef { get; set; }
    public TactEnum BottomTact { get; set; }
    public NoteCollection BottomNotes { get; set; }

    public Song? Song { get; set; }
}
