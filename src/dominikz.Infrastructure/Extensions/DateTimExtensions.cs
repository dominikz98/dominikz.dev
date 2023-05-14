using Microsoft.EntityFrameworkCore;

namespace dominikz.Infrastructure.Extensions;

public static class DateTimExtensions
{
    public static long ToUnixTimestamp(this DateTime source)
        => (long)(source - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

    public static (long Start, long End) ToUnixRange(this DateOnly source)
    {
        var start = (long)(source.ToDateTime(new TimeOnly(0, 0, 0, 0, 0)) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        var end = (long)(source.ToDateTime(new TimeOnly(23, 59, 59, 59, 59)) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        return (start, end);
    }


    public static DateTime ToLocalDateTime(this long source)
        => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(source).ToLocalTime();

    public static bool EqualsDateOnly(this DateTime first, DateTime second)
        => EF.Functions.Like(first.ToString("yyyy-MM-dd"), second.ToString("yyyy-MM-dd"));

    public static bool EqualsDateOnly(this DateTime? first, DateTime? second)
    {
        if (first is null && second is null)
            return true;

        if (first is null || second is null)
            return false;

        return first.Value.EqualsDateOnly(second.Value);
    }
}