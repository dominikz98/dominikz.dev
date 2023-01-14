namespace dominikz.Domain.Options;

public class MedlanOptions
{
    public string Url { get; set; } = string.Empty;
    public string ProjectUrl { get; set; } = string.Empty;
    public int CacheDurationInH { get; set; }
}