using dominikz.Domain.Contracts;

namespace dominikz.Domain.Filter;

public class EarningsCallsFilter : IFilter
{
    public DateOnly? Date { get; set; }

    public int? Start { get; set; }
    public int? Count { get; set; }

    public IReadOnlyCollection<FilterParam> GetParameter()
    {
        var result = new List<FilterParam>();

        if (Date != null)
            result.Add(new(nameof(Date), Date.Value.ToString()));

        return result;
    }
}