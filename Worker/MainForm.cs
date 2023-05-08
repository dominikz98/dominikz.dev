using Worker.Hubs;
using Worker.Utils;

namespace Worker;

public partial class MainForm : Form
{
    private readonly WorkerHost _host;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly TextBoxLoggerFactory _loggerFactory;

    public MainForm()
    {
        InitializeComponent();

        _cancellationTokenSource = new CancellationTokenSource();
        _loggerFactory = new TextBoxLoggerFactory(rtbGeneralLog);

        // TestQueueWorker.Init();
        _host = new WorkerHost(_loggerFactory, _cancellationTokenSource.Token);
        _host.Start().ConfigureAwait(false);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _cancellationTokenSource.Cancel();
        _host.Dispose();
        _loggerFactory.Dispose();
    }
}