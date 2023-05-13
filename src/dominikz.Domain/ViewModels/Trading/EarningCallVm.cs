using dominikz.Domain.Enums;

namespace dominikz.Domain.ViewModels.Trading;

public class EarningCallListVm
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public bool IsReleased { get; set; }
}

public class EarningCallVm : EarningCallListVm
{
    public string ISIN { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public EarningCallTime Time { get; set; }
    public DateTime Timestamp { get; set; }
    public decimal EpsActual { get; set; }
    public decimal EpsEstimate { get; set; }
    public long RevenueActual { get; set; }
    public long RevenueEstimate { get; set; }
    public decimal? LastStockPrice { get; set; }
    public DateTime? Updated { get; set; }
    public int Snapshots { get; set; }
    public TradeVm? Trade { get; set; }
    public MarketEvent[] MarketEvents { get; set; } = Array.Empty<MarketEvent>();
    public PriceSnapshot[] PriceSnapshots { get; set; } = Array.Empty<PriceSnapshot>();
    public BotEvent[] BotEvents { get; set; } = Array.Empty<BotEvent>();
    public EarningCallQuarter[] Quarters { get; set; } = Array.Empty<EarningCallQuarter>();
    public ExternalUrl[] Externals { get; set; } = Array.Empty<ExternalUrl>();
}

public class MarketEvent
{
    public DateTime Timestamp { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class PriceSnapshot
{
    public DateTime Timestamp { get; set; }
    public decimal Value { get; set; }
}

public class BotEvent
{
    public DateTime Timestamp { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class EarningCallQuarter
{
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class ExternalUrl
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}