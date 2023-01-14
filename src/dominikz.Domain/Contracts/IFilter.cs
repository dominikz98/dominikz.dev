namespace dominikz.Domain.Contracts;

public interface IFilter
{
    IReadOnlyCollection<FilterParam> GetParameter();
}

public class FilterParam
{
    public string Name { get; set; }
    public string Value { get; set; }

    public FilterParam(string name, string value)
    {
        Name = name;
        Value = value;
    }
}