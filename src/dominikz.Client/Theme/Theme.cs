using System.Text;

namespace dominikz.Client.Theme;

public class Theme
{
    public ThemeColor Surface { get; set; }
    public ThemeColor Background { get; set; }
    public ThemeColor Primary { get; set; }
    public ThemeColor OnPrimary { get => new(Primary.HexValue, 85); }
    public ThemeColor Secondary { get; set; }
    public ThemeColor OnSecondary { get => new(Secondary.HexValue, 85); }
    public ThemeColor Success { get; set; }
    public ThemeColor Warning { get; set; }
    public ThemeColor Error { get; set; }

    public Dictionary<string, string> ClassesByUrls = new()
    {
        { "/index", "page-index" },
        { "/blog", "page-blog" },
        { "/media", "page-media" },
        { "/music", "page-music" }
    };

    public Theme(string surface, string background, string primary, string secondary, string success, string warning, string error)
    {
        Surface = new(surface, 80);
        Background = new(background);
        Primary = new(primary);
        Secondary = new(secondary);
        Success = new(success);
        Warning = new(warning);
        Error = new(error);
    }

    public static Theme Dark => new("33383c", "000000", "5f7d8f", "FFFFFF", "179b27", "e3cb23", "a92425");

    public override string ToString()
    {
        var sb = new StringBuilder();

        // global variables
        sb.Append(":root{");
        sb.Append($"--theme-{nameof(Surface).ToLower()}:{Surface};");
        sb.Append($"--theme-{nameof(Background).ToLower()}:{Background};");
        sb.Append($"--theme-{nameof(Primary).ToLower()}:{Primary};");
        sb.Append($"--theme-{nameof(OnPrimary).ToLower()}:{OnPrimary};");
        sb.Append($"--theme-{nameof(Secondary).ToLower()}:{Secondary};");
        sb.Append($"--theme-{nameof(OnSecondary).ToLower()}:{OnSecondary};");
        sb.Append($"--theme-{nameof(Success).ToLower()}:{Success};");
        sb.Append($"--theme-{nameof(Warning).ToLower()}:{Warning};");
        sb.Append($"--theme-{nameof(Error).ToLower()}:{Error};");
        sb.Append("}");

        // disable scrollbars
        sb.Append("::-webkit-scrollbar{display: none;}");


        return sb.ToString();
    }
}

public struct ThemeColor
{
    public string HexValue { get; }
    public int Opacity { get; }

    public ThemeColor(string hex, int opacity = 100)
    {
        if (hex.Length != 6)
            throw new ArgumentException("Hex code needs to be passed without opacity and #!");

        if (opacity < 0 || opacity > 100)
            throw new ArgumentException("Opacity needs to be passed between 0 and 100!");

        Opacity = opacity;
        HexValue = hex;
    }

    public override string ToString()
    {
        var value = (int)Math.Min(Math.Round(((255 / (double)100) * Opacity), 0, MidpointRounding.AwayFromZero), 255);
        var opactiyHex = value.ToString("X");
        return $"#{HexValue[..6]}{opactiyHex}";
    }
}