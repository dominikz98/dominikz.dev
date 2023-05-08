using Microsoft.Extensions.Logging;

namespace Worker.Utils;

public class TextBoxLoggerFactory : ILoggerFactory
{
    private readonly RichTextBox _textBox;

    public TextBoxLoggerFactory(RichTextBox textBox)
    {
        _textBox = textBox;
    }

    public ILogger CreateLogger(string name)
    {
        return new TextBoxLogger(_textBox, name);
    }

    public void AddProvider(ILoggerProvider provider)
    {
        // Do nothing, since we don't need to add any providers
    }

    public void Dispose()
    {
        // Nothing to dispose
    }
}

public class TextBoxLogger : ILogger, IDisposable
{
    private readonly RichTextBox _textBox;
    private readonly string _name;

    public TextBoxLogger(RichTextBox textBox, string name)
    {
        _textBox = textBox;
        _name = name;
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return this; // Nothing to do here
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        // Log everything
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        // Write the log message to the textbox
        var color = logLevel switch
        {
            LogLevel.Trace or LogLevel.Debug => Color.DarkGray,
            LogLevel.Warning => Color.Orange,
            LogLevel.Error or LogLevel.Critical => Color.Red,
            _ => Color.White
        };

        _textBox.AppendText($"[{DateTime.Now:HH:mm:ss}] [{_name}]: {formatter(state, exception)}{Environment.NewLine}", color);
    }

    public void Dispose()
    {
    }
}