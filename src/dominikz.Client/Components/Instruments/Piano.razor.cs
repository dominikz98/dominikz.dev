using dominikz.Domain.ViewModels.Songs;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace dominikz.Client.Components.Instruments;

public partial class Piano
{
    [Parameter] public EventCallback<List<NoteArgs>> NoteStarted { get; set; }
    [Parameter] public EventCallback<List<NoteArgs>> NoteStopped { get; set; }

    [Inject] protected IJSRuntime? JsRuntime { get; set; }

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

    private readonly List<PianoKey> _keyRefs = new();

    protected PianoKey? KeyRef
    {
        set => _keyRefs.Add(value!);
    }

    private List<Tone> _playedTones = new();
    private PianoController? _controller;

    private void Init()
    {
        if (Song is null)
            return;

        _controller = new PianoController(Song);
        _controller.NoteStarted += (_, args) =>
        {
            PlayNotes(args);
            NoteStarted.InvokeAsync(args).ConfigureAwait(false);
        };
        _controller.NoteStopped += (_, args) => NoteStopped.InvokeAsync(args).ConfigureAwait(false);
        _controller.SongFinished += (_, _) => StateHasChanged();
    }

    public void Play()
        => _controller?.Play();

    public void Stop()
        => _controller?.Stop();

    public void Pause()
        => _controller?.Pause();

    private void PlayNotes(List<NoteArgs> args)
    {
        foreach (var arg in args)
            PlayNote(Tone.FromNote(arg.Note));
    }

    private async void PlayNote(Tone tone)
    {
        // select piano key
        var pianoKey = _keyRefs.First(x => x.Tone == tone);
        pianoKey.Select();

        // play 
        await JsRuntime!.InvokeVoidAsync("play", $"assets/piano/{tone.Note}{tone.Segment}.mp3");

        // add to note history
        _playedTones.Add(tone);
        StateHasChanged();

        DeselectKeyDelayed(pianoKey);
    }

    private async Task CallVolumeChanged(int volume)
        => await JsRuntime!.InvokeVoidAsync("setVolume", (double)volume / 100);

    private async void DeselectKeyDelayed(PianoKey key)
    {
        await Task.Delay(300);
        key.DeSelect();
    }
}