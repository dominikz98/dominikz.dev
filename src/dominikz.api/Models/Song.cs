namespace dominikz.api.Models;

public class Song
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int BPM { get; set; }

    public IReadOnlyCollection<SongSegment> Segments { get; set; } = new List<SongSegment>();
}
