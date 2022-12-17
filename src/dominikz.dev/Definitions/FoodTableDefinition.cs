using dominikz.dev.Components.Tables;
using dominikz.dev.Utils;
using dominikz.shared.ViewModels;

namespace dominikz.dev.Definitions;

public static class FoodTableDefinition
{
    public static List<ColumnDefinition<FoodDetailVM>> Columns
    {
        get => new()
        {
            new(nameof(FoodDetailVM.Title), (x) => x.Title),
            new(nameof(FoodDetailVM.Unit), (x) => $"{x.Multiplier * x.Count:0} {EnumConverter.ToString(x.Unit)}"),
            new("x", (x) => $"({x.Multiplier:0.####})") { Actions = ColumnActionFlags.HIDE_ON_MOBILE },
            new("Price", (x) => x.Multiplier * x.PricePerCount) { Formatter = (x) => $"{x:c2}", Actions = ColumnActionFlags.SUM },
            new("Link", (x) => x.ReweUrl) { Actions = ColumnActionFlags.LINK  | ColumnActionFlags.HIDE_ON_MOBILE }
        };
    }
}