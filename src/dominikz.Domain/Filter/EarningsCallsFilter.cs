using dominikz.Domain.Contracts;

namespace dominikz.Domain.Filter;

public class EarningsCallsFilter : IFilter
{
    public bool OnlyIncreased { get; set; }
    public string? Text { get; set; }
    public DateOnly? Date { get; set; }
    
    public int? Start { get; set; }
    public int? Count { get; set; }

    public IReadOnlyCollection<FilterParam> GetParameter()
    {
        var result = new List<FilterParam>();

        if (OnlyIncreased == true)
            result.Add(new(nameof(OnlyIncreased), OnlyIncreased.ToString()));

        if (Date != null)
            result.Add(new(nameof(Date), Date.Value.ToString()));
        
        if (string.IsNullOrWhiteSpace(Text) == false)
            result.Add(new(nameof(Text), Text));

        return result;
    }
}