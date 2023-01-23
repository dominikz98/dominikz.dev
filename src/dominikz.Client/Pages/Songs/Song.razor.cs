using dominikz.Client.Components.Instruments;
using dominikz.Domain.ViewModels.Songs;
using dominikz.Infrastructure.Clients.Api;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Pages.Songs;

public partial class Song
{
    [Parameter] public Guid? SongId { get; set; }
    [Inject] protected SongsEndpoints? Endpoints { get; set; }
    private SongVm? _song;
    private Piano? _piano;
    private SongChart? _chart;

    protected override async Task OnInitializedAsync()
    {
        if (SongId == null)
            return;

        _song = await Endpoints!.GetById(SongId.Value);
        Console.WriteLine("Song loaded!");
    }

    private void OnNoteChanged(List<NoteArgs> notes)
    {
        foreach (var note in notes)
            _chart?.ToggleSelect(note.SegmentIndex, note.LaneIndex, Tone.FromNote(note.Note));
    }
}