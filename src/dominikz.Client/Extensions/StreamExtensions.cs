namespace dominikz.Client.Extensions;

public static class StreamExtensions
{
    public static async Task<byte[]> ReadBytesAsync(this Stream stream)
    {
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        return ms.ToArray();
    }
}