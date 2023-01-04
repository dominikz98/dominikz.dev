namespace dominikz.dev.Models;

public struct TextStruct
{
    private Guid _id;
    public string Text { get; }

    public TextStruct(string text)
    {
        _id = Guid.NewGuid();
        Text = text;
    }
}