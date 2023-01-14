namespace dominikz.Client.Components.Tables;

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
    None = 0,
    Sum = 1,
    Link = 2,
    HideOnMobile = 4
}