using dominikz.shared.ViewModels;

namespace dominikz.dev.Components.Instruments;

internal class PianoController
{
    public event EventHandler<NoteEventArgs>? OnNoteStarted;
    public event EventHandler<NoteEventArgs>? OnNoteStopped;
    public bool Playing;

    private readonly List<LaneController> _topCtrls = new();
    private readonly List<LaneController> _bottomCtrls = new();

    public PianoController(SongVm song)
    {
        if (song.Top.Count != song.Bottom.Count)
            throw new ArgumentException("top- and bottom lane count mismatch!");

        // create top lane controller
        for (int i = 0; i < song.Top.Count; i++)
        {
            var ctrl = new LaneController(0, i, song.Top[i]);
            ctrl.OnNoteStarted += (s, e) => OnNoteStarted?.Invoke(s, e);
            ctrl.OnNoteStopped += (s, e) => OnNoteStopped?.Invoke(s, e);
            ctrl.OnLaneFinished += OnLaneFinished;
            _topCtrls.Add(ctrl);
        }

        // create bottom lane controller
        for (int i = 0; i < song.Bottom.Count; i++)
        {
            var ctrl = new LaneController(1, i, song.Bottom[i]);
            ctrl.OnNoteStarted += (s, e) => OnNoteStarted?.Invoke(s, e);
            ctrl.OnNoteStopped += (s, e) => OnNoteStopped?.Invoke(s, e);
            ctrl.OnLaneFinished += OnLaneFinished;
            _bottomCtrls.Add(ctrl);
        }
    }

    private void OnLaneFinished(object? sender, LaneEventArgs e)
    {
        var anyLaneUnfinishedYet = _topCtrls[e.LaneIndex].Playing || _bottomCtrls[e.LaneIndex].Playing;
        if (anyLaneUnfinishedYet)
            return;

        // stop
        var idx = e.LaneIndex + 1;
        if (idx == _topCtrls.Count)
        {
            Playing = false;
            return;
        }

        // call lanes in next segment
        _topCtrls[idx].Play();
        _bottomCtrls[idx].Play();
    }

    public void Play()
    {
        Playing = true;
        _topCtrls[0].Play();
        _bottomCtrls[0].Play();
    }
}
