using dominikz.Domain.Enums.Music;
using dominikz.Domain.Structs;
using dominikz.Domain.ViewModels.Songs;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Pages.Songs;

public partial class EditLane
{
    [Parameter] public LaneVm? Value { get; set; }
    
    protected int Positions
    {
        get => Value?.Notes.Count ?? 0;
        set
        {
            if (Value == null)
                return;
            
            Value.Notes = new List<NoteVm>();
            for (var i = 0; i < value; i++)
                Value.Notes.Add(new NoteVm()
                {
                    Position = 128 / value * i,
                    Note = NoteEnum.C,
                    Segment = 4,
                    Type = NoteTypeEnum.Quarter
                });
        }
    }

    private string _code = string.Empty;

    private string GetLaneStyle(NoteVm note, int segment, NoteEnum type)
        => note.Note == type && note.Segment == segment
            ? "song-lane-selected"
            : string.Empty;

    private void GenerateCode()
    {
        if (Value == null)
            return;
        
        var notes = Value.Notes.Select(x => new NoteData(x.Type, x.Note, x.Segment, x.Position)).ToList();
        _code = new NoteCollection(notes).ToString();
        StateHasChanged();
    }
}