using System.Security.Cryptography;
using System.Text;

namespace dominikz.Client.Utils;

public static class Colorizer
{
    public static string GetColoredNumberValue(decimal? value)
    {
        if ((value ?? 0) > 0)
            return "var(--theme-success)";
        
        if ((value ?? 0) < 0)
            return "var(--theme-error)";

        return string.Empty;
    }
    
    public static string CreateColorDarkByText(string text)
    {
        using var sha256 = SHA256.Create();
        var inputBytes = Encoding.UTF8.GetBytes(text);
        var hashBytes = sha256.ComputeHash(inputBytes);

        // Use the first three bytes of the hash as RGB values
        var r = hashBytes[0];
        var g = hashBytes[1];
        var b = hashBytes[2];

        // Calculate the luminance value to determine if the color is "dark enough"
        var luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255;

        // If the color is not dark enough, adjust the values
        if (luminance > 0.5) {
            var adjustmentFactor = 0.5 / luminance;
            r = (byte)Math.Round(r * adjustmentFactor);
            g = (byte)Math.Round(g * adjustmentFactor);
            b = (byte)Math.Round(b * adjustmentFactor);
        }

        // Convert the RGB values to a hex color code
        return $"#{r:X2}{g:X2}{b:X2}";
    }
}