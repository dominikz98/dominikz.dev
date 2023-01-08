using System.Diagnostics.CodeAnalysis;
using dominikz.shared.Enums;

namespace dominikz.api.Models.Structs;

public readonly struct NoteData
{
    public int Position { get; }
    public NoteTypeEnum Type { get; }
    public NoteEnum Note { get; }
    public int Segment { get; }

    public NoteData(NoteTypeEnum type, NoteEnum note, int segment, int position)
    {
        Type = type;
        Note = note;
        Segment = segment;
        Position = position;
    }

    public NoteData(string key, int position)
    {
        if (key.Length < 3)
            throw new ArgumentException("Invalid note key!");

        if (!int.TryParse(key[0].ToString(), out var segment))
            throw new ArgumentException("Invalid segment!");

        if (!Enum.TryParse<NoteEnum>(key[1].ToString(), out var note))
            throw new ArgumentException("Invalid note!");

        if (!Enum.TryParse<NoteTypeEnum>(key[2..], out var type))
            throw new ArgumentException("Invalid type!");

        Segment = segment;
        Type = type;
        Note = note;
        Position = position;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is not null && obj is NoteData data && data == this;

    public override int GetHashCode()
        => Segment.GetHashCode() ^ Type.GetHashCode() ^ Note.GetHashCode();

    public static bool operator ==(NoteData x, NoteData y)
        => x.Segment == y.Segment && x.Type == y.Type && x.Note == y.Note;

    public static bool operator !=(NoteData x, NoteData y)
        => x.Segment != y.Segment || x.Type != y.Type || x.Note != y.Note;

    /// <summary>
    /// Structure: Segment | Type | Note
    /// Example: Segment:4; Note:G; Type:Quarter; -> 4G4
    /// </summary>
    public override string ToString()
        => $"{Segment}{Note}{(int)Type}";
}
