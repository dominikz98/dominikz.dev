namespace dominikz.Worker.Utils;

public static class DateTimeExtensions
{
    public static bool IsLowerOrEqualWithoutSeconds(this DateTime first, DateTime second)
    {
        var firstTmp = new DateTime(first.Year, first.Month, first.Day, first.Hour, first.Minute, 0);
        var secondTmp = new DateTime(second.Year, second.Month, second.Day, second.Hour, second.Minute, 0);
        return firstTmp <= secondTmp;
    }
}