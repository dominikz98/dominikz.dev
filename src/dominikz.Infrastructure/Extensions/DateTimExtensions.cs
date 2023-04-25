namespace dominikz.Infrastructure.Extensions;

public static class DateTimExtensions
{
    public static long ToUnixTimestamp(this DateTime source)
        => (long)(source - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    
    public static DateTime FromUnixTimestamp(this int source)
        => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(source);
}