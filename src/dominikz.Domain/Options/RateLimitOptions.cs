using System.Diagnostics.CodeAnalysis;

namespace dominikz.Domain.Options;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class RateLimitOptions
{
    public int TokenLimit { get; set; }
    public int ReplenishmentPeriodInS { get; set; }
    public int TokensPerPeriod { get; set; }
    public bool AutoReplenishment { get; set; }
}