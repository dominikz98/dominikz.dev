using System.Diagnostics;

namespace dominikz.Infrastructure.Utils;

// ReSharper disable once InconsistentNaming
public static class FFMPEGWrapper
{
    public static IDictionary<string, string> GetMatadata(string filepath)
    {
        var psi = new ProcessStartInfo("ffmpeg", "-i \"" + filepath + "\" -f ffmetadata -")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false
        };

        using var process = Process.Start(psi);
        if (process == null)
            return new Dictionary<string, string>();

        var output = process.StandardOutput.ReadToEnd();

        // Parse the output to extract the metadata
        var lines = output.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Where(x => x != "\\")
            .Select(x => x.Trim())
            .ToList();

        var keys = new List<string>()
        {
            "Major_Brand",
            "Minor_Version",
            "Compatible_Brands",
            "Title",
            "Artist",
            "Date",
            "Synopsis",
            "Comment",
            "Description",
            "Encoder"
        };
        var result = new Dictionary<string, string>();
        foreach (var line in lines)
        {
            if (line.StartsWith(';'))
                continue;
            
            var parts = line.Split('=').ToList();
            var key = keys.FirstOrDefault(x => parts[0].StartsWith(x, StringComparison.OrdinalIgnoreCase));
            if (key != null 
                && parts.Count > 1 
                && result.ContainsKey(key) == false)
            {
                result.Add(key, string.Join('=', parts.GetRange(1, parts.Count - 1)));
                continue;
            }

            if (result.Keys.Count == 0)
            {
                result.Add(Guid.NewGuid().ToString(), line);
                continue;
            }

            result[result.Keys.Last()] += Environment.NewLine + line;
        }

        return result;
    }
}