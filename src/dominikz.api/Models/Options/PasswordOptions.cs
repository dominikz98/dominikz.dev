namespace dominikz.api.Models.Options;

public class PasswordOptions
{
    public int SaltLength { get; set; }
    public int KeyLength { get; set; }
    public int IterationCount { get; set; }
    public string Pepper { get; set; } = string.Empty;
}