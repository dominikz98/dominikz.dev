namespace dominikz.Domain.Enums.Trades;

[Flags]
public enum InformationSource
{
    EarningsWhispers = 1,
    OnVista = 2,
    FinanzenNet = 4,
    AktienFinder = 8
}