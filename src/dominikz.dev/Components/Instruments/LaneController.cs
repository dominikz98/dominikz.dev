using System.Timers;
using dominikz.shared.ViewModels;
using Timer = System.Timers.Timer;

namespace dominikz.dev.Components.Instruments;

internal class LaneController
{
    public event EventHandler<LaneEventArgs>? OnLaneFinished;
    public event EventHandler<NoteEventArgs>? OnNoteStarted;
    public event EventHandler<NoteEventArgs>? OnNoteStopped;
    public bool Playing;

    private readonly int _posIndex;
    private readonly int _laneIndex;
    private readonly int _segmentIndex;
    private readonly int _availableTicks;
    private readonly IReadOnlyCollection<NoteVm> _notes;
    private readonly Timer _timer;
    private int _tick = 0;

    public LaneController(int posIndex, int laneIndex, LaneVm lane)
    {
        _posIndex = posIndex;
        _laneIndex = laneIndex;
        _segmentIndex = lane.SegmentIndex;
        _availableTicks = lane.AvailableTicks;
        _notes = lane.Notes.OrderBy(x => x.Position).ToList();

        _timer = new Timer(lane.TickDurationInMs);
        _timer.Elapsed += TickElapsed;
    }

    public void Play()
    {
        Playing = true;
        _timer.Start();
    }

    private void TickElapsed(object? sender, ElapsedEventArgs e)
    {
        // stop previous note
        var stoppNotes = _notes.Where(x => x.Position + x.Ticks == _tick).ToList();
        foreach (var note in stoppNotes)
            OnNoteStopped?.Invoke(this, new NoteEventArgs(_posIndex, _laneIndex, _segmentIndex, note));

        // trigger new note
        var startNotes = _notes.Where(x => x.Position == _tick).ToList();
        foreach (var note in startNotes)
            OnNoteStarted?.Invoke(this, new NoteEventArgs(_posIndex, _laneIndex, _segmentIndex, note));

        _tick++;

        // exit
        if (_tick < _availableTicks)
            return;

        _timer!.Stop();
        Playing = false;
        OnLaneFinished?.Invoke(this, new LaneEventArgs(_posIndex, _laneIndex, _segmentIndex));
    }
}

internal class NoteEventArgs : LaneEventArgs
{
    public NoteEventArgs(int posIndex, int laneIndex, int segmentIndex, NoteVm note) : base(posIndex, laneIndex, segmentIndex)
    {
        Note = note;
    }

    public NoteVm Note { get; }
}

internal class LaneEventArgs : EventArgs
{
    public LaneEventArgs(int posIndex, int laneIndex, int segmentIndex)
    {
        PosIndex = posIndex;
        LaneIndex = laneIndex;
        SegmentIndex = segmentIndex;
    }

    public int PosIndex { get; }
    public int LaneIndex { get; }
    public int SegmentIndex { get; }
}
