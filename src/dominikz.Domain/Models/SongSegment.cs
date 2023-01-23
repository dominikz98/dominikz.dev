using dominikz.Domain.Enums.Music;
using dominikz.Domain.Structs;

namespace dominikz.Domain.Models;

public class SongSegment
{
    public int Index { get; set; }
    public Guid SongId { get; set; }

    // Top
    public ClefEnum TopClef { get; set; }
    public TactEnum TopTact { get; set; }
    public NoteCollection TopNotes { get; set; }
    
    // Bottom
    public ClefEnum BottomClef { get; set; }
    public TactEnum BottomTact { get; set; }
    public NoteCollection BottomNotes { get; set; }
    
    public Song? Song { get; set; }
}