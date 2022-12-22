using dominikz.dev.Utils;
using dominikz.shared.ViewModels;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Components.Instruments;

public partial class Piano
{
    private SongVm? _song;
    [Parameter]
#pragma warning disable BL0007
    public SongVm? Song
#pragma warning restore BL0007
    {
        get => _song;
        set
        {
            _song = value;
            Init();
        }
    }

    [Inject]
    protected BrowserService? Browser { get; set; }

    private readonly List<PianoKey> _keyRefs = new();
    protected PianoKey? KeyRef { set => _keyRefs.Add(value!); }

    private PianoController? _controller;
    private readonly Dictionary<string, int> _audioAssignments = new();

    private void Init()
    {
        if (Song is null)
            return;

        _controller = new PianoController(Song);
        _controller.OnNoteStarted += Controller_OnNoteStarted;
        _controller.OnNoteStopped += Controller_OnNoteStopped;
    }

    public void Play()
        => _controller?.Play();

    private void OnKeyClicked(Tone tone)
    {

    }

    private async void Controller_OnNoteStopped(object? sender, NoteEventArgs args)
    {
        // create and log note key
        var key = CreateNoteKey(args);
        var audioId = _audioAssignments[key];
        Console.WriteLine($"[{DateTime.Now}] Remove: Channel={audioId}; Key={key}");

        // deselect piano key
        var pianoKey = _keyRefs.First(x => x.Tone == Tone.FromNote(args.Note));
        pianoKey.DeSelect();

        // free audio channel
        _audioAssignments.Remove(key);
        await Browser!.StopAudio($"a{audioId}");
    }

    private async void Controller_OnNoteStarted(object? sender, NoteEventArgs args)
    {
        // get available audio channels
        var occupiedIds = _audioAssignments.Values.Select(x => x) ?? new List<int>();
        var availableIds = Enumerable.Range(1, 14);
        var audioId = availableIds.Except(occupiedIds).First();

        // create and log note key
        var noteKey = CreateNoteKey(args);
        Console.WriteLine($"[{DateTime.Now}] Add: Channel={audioId}; Key={noteKey}");

        // select piano key
        var pianoKey = _keyRefs.First(x => x.Tone == Tone.FromNote(args.Note));
        pianoKey.Select();

        // register and play audio channel
        _audioAssignments.TryAdd(noteKey, audioId);
        await Browser!.PlayAudio($"a{audioId}", $"assets/piano/{args.Note.Note}{args.Note.Segment}.mp3");
    }

    private static string CreateNoteKey(NoteEventArgs args)
        => $"PIX{args.PosIndex}LIX{args.LaneIndex}SIX{args.SegmentIndex}S{args.Note.Segment}T{args.Note.Type}N{args.Note.Note}P{args.Note.Position}";
}
