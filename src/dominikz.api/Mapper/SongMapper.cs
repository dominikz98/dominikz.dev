using dominikz.api.Models;
using dominikz.shared.Enums;
using dominikz.shared.ViewModels;

namespace dominikz.api.Mapper;

public static class SongMapper
{
    public static SongVm MapToVm(this Song song)
        => new()
        {
            Id = song.Id,
            BPM = song.BPM,
            Name = song.Name,
            Top = song.Segments.Select(x => new LaneVm()
            {
                SegmentIndex = x.Index,
                Clef = x.TopClef,
                Tact = x.TopTact,
                AvailableTicks = GetAvailableTicksByTact(x.TopTact),
                TickDurationInMs = GetTickDurationByTactAndBpm(x.TopTact, song.BPM),
                Notes = x.TopNotes.Notes.Select(y => new NoteVm()
                {
                    Position = y.Position,
                    Note = y.Note,
                    Segment = y.Segment,
                    Type = y.Type,
                    Ticks = GetTicksByTactAndType(x.TopTact, y.Type)
                }).ToList()
            }).ToList(),
            Bottom = song.Segments.Select(x => new LaneVm()
            {
                SegmentIndex = x.Index,
                Clef = x.BottomClef,
                Tact = x.BottomTact,
                AvailableTicks = GetAvailableTicksByTact(x.BottomTact),
                TickDurationInMs = GetTickDurationByTactAndBpm(x.BottomTact, song.BPM),
                Notes = x.BottomNotes.Notes.Select(y => new NoteVm()
                {
                    Position = y.Position,
                    Note = y.Note,
                    Segment = y.Segment,
                    Type = y.Type,
                    Ticks = GetTicksByTactAndType(x.BottomTact, y.Type)
                }).ToList()
            }).ToList()
        };

    private static int GetTicksByTactAndType(TactEnum tact, NoteTypeEnum note)
    {
        var available = GetAvailableTicksByTact(tact);
        return available / ((int)note);
    }

    private static int GetTickDurationByTactAndBpm(TactEnum tact, int bpm)
    {
        var tactDuration = GetTactDurationByTactAndBpm(tact, bpm);
        return tactDuration / ((int)NoteTypeEnum.ThirtySecond) / ((int)tact);
    }

    private static int GetTactDurationByTactAndBpm(TactEnum tact, int bpm)
    {
        var beatsPerTactPerMin = 60 * ((int)tact);
        return (int)(beatsPerTactPerMin / (double)bpm * 1000);
    }

    private static int GetAvailableTicksByTact(TactEnum tact)
        => ((int)tact) * ((int)NoteTypeEnum.ThirtySecond);
}
