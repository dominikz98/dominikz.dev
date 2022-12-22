using System.Diagnostics.CodeAnalysis;
using dominikz.shared.Contracts;
using dominikz.shared.ViewModels;

namespace dominikz.dev.Components.Instruments;

public struct Tone
{
    public NoteEnum Note { get; set; }
    public int Segment { get; set; }
    public bool IsGroupTone { get => Note.ToString().Length > 1; }

    public Tone(NoteEnum note, int segment)
    {
        Note = note;
        Segment = segment;
    }

    public static readonly Tone A0 = new(NoteEnum.A, 0);
    public static readonly Tone A1 = new(NoteEnum.A, 1);
    public static readonly Tone A2 = new(NoteEnum.A, 2);
    public static readonly Tone A3 = new(NoteEnum.A, 3);
    public static readonly Tone A4 = new(NoteEnum.A, 4);
    public static readonly Tone A5 = new(NoteEnum.A, 5);
    public static readonly Tone A6 = new(NoteEnum.A, 6);
    public static readonly Tone A7 = new(NoteEnum.A, 7);
    public static readonly Tone AB1 = new(NoteEnum.AB, 1);
    public static readonly Tone AB2 = new(NoteEnum.AB, 2);
    public static readonly Tone AB3 = new(NoteEnum.AB, 3);
    public static readonly Tone AB4 = new(NoteEnum.AB, 4);
    public static readonly Tone AB5 = new(NoteEnum.AB, 5);
    public static readonly Tone AB6 = new(NoteEnum.AB, 6);
    public static readonly Tone AB7 = new(NoteEnum.AB, 7);
    public static readonly Tone B0 = new(NoteEnum.B, 0);
    public static readonly Tone B1 = new(NoteEnum.B, 1);
    public static readonly Tone B2 = new(NoteEnum.B, 2);
    public static readonly Tone B3 = new(NoteEnum.B, 3);
    public static readonly Tone B4 = new(NoteEnum.B, 4);
    public static readonly Tone B5 = new(NoteEnum.B, 5);
    public static readonly Tone B6 = new(NoteEnum.B, 6);
    public static readonly Tone B7 = new(NoteEnum.B, 7);
    public static readonly Tone BB0 = new(NoteEnum.BB, 0);
    public static readonly Tone BB1 = new(NoteEnum.BB, 1);
    public static readonly Tone BB2 = new(NoteEnum.BB, 2);
    public static readonly Tone BB3 = new(NoteEnum.BB, 3);
    public static readonly Tone BB4 = new(NoteEnum.BB, 4);
    public static readonly Tone BB5 = new(NoteEnum.BB, 5);
    public static readonly Tone BB6 = new(NoteEnum.BB, 6);
    public static readonly Tone BB7 = new(NoteEnum.BB, 7);
    public static readonly Tone C1 = new(NoteEnum.C, 1);
    public static readonly Tone C2 = new(NoteEnum.C, 2);
    public static readonly Tone C3 = new(NoteEnum.C, 3);
    public static readonly Tone C4 = new(NoteEnum.C, 4);
    public static readonly Tone C5 = new(NoteEnum.C, 5);
    public static readonly Tone C6 = new(NoteEnum.C, 6);
    public static readonly Tone C7 = new(NoteEnum.C, 7);
    public static readonly Tone C8 = new(NoteEnum.C, 8);
    public static readonly Tone D1 = new(NoteEnum.D, 1);
    public static readonly Tone D2 = new(NoteEnum.D, 2);
    public static readonly Tone D3 = new(NoteEnum.D, 3);
    public static readonly Tone D4 = new(NoteEnum.D, 4);
    public static readonly Tone D5 = new(NoteEnum.D, 5);
    public static readonly Tone D6 = new(NoteEnum.D, 6);
    public static readonly Tone D7 = new(NoteEnum.D, 7);
    public static readonly Tone DB1 = new(NoteEnum.DB, 1);
    public static readonly Tone DB2 = new(NoteEnum.DB, 2);
    public static readonly Tone DB3 = new(NoteEnum.DB, 3);
    public static readonly Tone DB4 = new(NoteEnum.DB, 4);
    public static readonly Tone DB5 = new(NoteEnum.DB, 5);
    public static readonly Tone DB6 = new(NoteEnum.DB, 6);
    public static readonly Tone DB7 = new(NoteEnum.DB, 7);
    public static readonly Tone E1 = new(NoteEnum.E, 1);
    public static readonly Tone E2 = new(NoteEnum.E, 2);
    public static readonly Tone E3 = new(NoteEnum.E, 3);
    public static readonly Tone E4 = new(NoteEnum.E, 4);
    public static readonly Tone E5 = new(NoteEnum.E, 5);
    public static readonly Tone E6 = new(NoteEnum.E, 6);
    public static readonly Tone E7 = new(NoteEnum.E, 7);
    public static readonly Tone EB1 = new(NoteEnum.EB, 1);
    public static readonly Tone EB2 = new(NoteEnum.EB, 2);
    public static readonly Tone EB3 = new(NoteEnum.EB, 3);
    public static readonly Tone EB4 = new(NoteEnum.EB, 4);
    public static readonly Tone EB5 = new(NoteEnum.EB, 5);
    public static readonly Tone EB6 = new(NoteEnum.EB, 6);
    public static readonly Tone EB7 = new(NoteEnum.EB, 7);
    public static readonly Tone F1 = new(NoteEnum.F, 1);
    public static readonly Tone F2 = new(NoteEnum.F, 2);
    public static readonly Tone F3 = new(NoteEnum.F, 3);
    public static readonly Tone F4 = new(NoteEnum.F, 4);
    public static readonly Tone F5 = new(NoteEnum.F, 5);
    public static readonly Tone F6 = new(NoteEnum.F, 6);
    public static readonly Tone F7 = new(NoteEnum.F, 7);
    public static readonly Tone G1 = new(NoteEnum.G, 1);
    public static readonly Tone G2 = new(NoteEnum.G, 2);
    public static readonly Tone G3 = new(NoteEnum.G, 3);
    public static readonly Tone G4 = new(NoteEnum.G, 4);
    public static readonly Tone G5 = new(NoteEnum.G, 5);
    public static readonly Tone G6 = new(NoteEnum.G, 6);
    public static readonly Tone G7 = new(NoteEnum.G, 7);
    public static readonly Tone GB1 = new(NoteEnum.GB, 1);
    public static readonly Tone GB2 = new(NoteEnum.GB, 2);
    public static readonly Tone GB3 = new(NoteEnum.GB, 3);
    public static readonly Tone GB4 = new(NoteEnum.GB, 4);
    public static readonly Tone GB5 = new(NoteEnum.GB, 5);
    public static readonly Tone GB6 = new(NoteEnum.GB, 6);
    public static readonly Tone GB7 = new(NoteEnum.GB, 7);

    public static Tone FromNote(NoteVm note)
        => new() { Note = note.Note, Segment = note.Segment };

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is not null && obj is Tone data && data == this;

    public override int GetHashCode()
        => Segment.GetHashCode() ^ Note.GetHashCode();

    public static bool operator ==(Tone x, Tone y)
        => x.Segment == y.Segment && x.Note == y.Note;

    public static bool operator !=(Tone x, Tone y)
        => x.Segment != y.Segment || x.Note != y.Note;

}