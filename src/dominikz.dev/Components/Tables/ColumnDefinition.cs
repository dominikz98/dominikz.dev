namespace dominikz.dev.Components.Tables;

public class ColumnDefinition<T>
{
    public string? Name { get; private set; }
    public Func<T, object?> Accessor { get; private set; }
    public Func<object?, string> Formatter { get; set; } = (x) => x?.ToString() ?? string.Empty;

    public ColumnActionFlags Actions { get; set; }

    public ColumnDefinition(string name, Func<T, object?> accessor)
    {
        Name = name;
        Accessor = accessor;
    }
}

[Flags]
public enum ColumnActionFlags
{
    NONE = 0,
    SUM = 1,
    LINK = 2,
    HIDE_ON_MOBILE = 4
}