using System.Text.RegularExpressions;

namespace dominikz.Domain.Structs;

public struct CronSchedule
{
    static readonly Regex DividedRegex = new(@"(\*/\d+)");
    static readonly Regex RangeRegex = new(@"(\d+\-\d+)\/?(\d+)?");
    static readonly Regex WildRegex = new(@"(\*)");
    static readonly Regex ListRegex = new(@"(((\d+,)*\d+)+)");
    static readonly Regex ValidationRegex = new(DividedRegex + "|" + RangeRegex + "|" + WildRegex + "|" + ListRegex);

    private readonly string _expression;
    private List<int> _minutes = new();
    private List<int> _hours = new();
    private List<int> _daysOfMonth = new();
    private List<int> _months = new();
    private List<int> _daysOfWeek = new();

    public CronSchedule(string expressions)
    {
        _expression = expressions;
        Generate();
    }

    private bool IsValid()
        => ValidationRegex.Matches(_expression).Count > 0;

    public bool IsTime(DateTime dateTime)
        => _minutes.Contains(dateTime.Minute) &&
           _hours.Contains(dateTime.Hour) &&
           _daysOfMonth.Contains(dateTime.Day) &&
           _months.Contains(dateTime.Month) &&
           _daysOfWeek.Contains((int)dateTime.DayOfWeek);

    private void Generate()
    {
        if (!IsValid()) return;

        var matches = ValidationRegex.Matches(_expression);

        GenerateMinutes(matches[0].ToString());

        if (matches.Count > 1)
            GenerateHours(matches[1].ToString());
        else
            GenerateHours("*");

        if (matches.Count > 2)
            GenerateDaysOfMonth(matches[2].ToString());
        else
            GenerateDaysOfMonth("*");

        if (matches.Count > 3)
            GenerateMonths(matches[3].ToString());
        else
            GenerateMonths("*");

        if (matches.Count > 4)
            GenerateDaysOfWeeks(matches[4].ToString());
        else
            GenerateDaysOfWeeks("*");
    }

    private void GenerateMinutes(string match)
        => _minutes = GenerateValues(match, 0, 60);

    private void GenerateHours(string match)
        => _hours = GenerateValues(match, 0, 24);

    private void GenerateDaysOfMonth(string match)
        => _daysOfMonth = GenerateValues(match, 1, 32);

    private void GenerateMonths(string match)
        => _months = GenerateValues(match, 1, 13);

    private void GenerateDaysOfWeeks(string match)
        => _daysOfWeek = GenerateValues(match, 0, 7);

    private List<int> GenerateValues(string configuration, int start, int max)
    {
        if (DividedRegex.IsMatch(configuration)) return divided_array(configuration, start, max);
        if (RangeRegex.IsMatch(configuration)) return RangeArray(configuration);
        if (WildRegex.IsMatch(configuration)) return WildArray(configuration, start, max);
        if (ListRegex.IsMatch(configuration)) return ListArray(configuration);

        return new List<int>();
    }

    private List<int> divided_array(string configuration, int start, int max)
    {
        if (!DividedRegex.IsMatch(configuration))
            return new List<int>();

        var ret = new List<int>();
        string[] split = configuration.Split("/".ToCharArray());
        var divisor = int.Parse(split[1]);

        for (var i = start; i < max; ++i)
            if (i % divisor == 0)
                ret.Add(i);

        return ret;
    }

    private List<int> RangeArray(string configuration)
    {
        if (!RangeRegex.IsMatch(configuration))
            return new List<int>();

        var ret = new List<int>();
        var split = configuration.Split("-".ToCharArray());
        var start = int.Parse(split[0]);
        int end;
        if (split[1].Contains("/"))
        {
            split = split[1].Split("/".ToCharArray());
            end = int.Parse(split[0]);
            var divisor = int.Parse(split[1]);

            for (var i = start; i < end; ++i)
                if (i % divisor == 0)
                    ret.Add(i);
            return ret;
        }
        else
            end = int.Parse(split[1]);

        for (var i = start; i <= end; ++i)
            ret.Add(i);

        return ret;
    }

    private List<int> WildArray(string configuration, int start, int max)
    {
        if (!WildRegex.IsMatch(configuration))
            return new List<int>();

        var ret = new List<int>();

        for (var i = start; i < max; ++i)
            ret.Add(i);

        return ret;
    }

    private List<int> ListArray(string configuration)
    {
        if (!ListRegex.IsMatch(configuration))
            return new List<int>();

        var ret = new List<int>();

        string[] split = configuration.Split(",".ToCharArray());

        foreach (var s in split)
            ret.Add(int.Parse(s));

        return ret;
    }
}