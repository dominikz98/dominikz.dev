using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace dominikz.Infrastructure.Provider.Database.Converter;

public class QuarterConverter : ValueConverter<decimal[], string>
{
    public QuarterConverter()
        : base(x => string.Join(';', x.Select(y => y)),
            x => x.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(decimal.Parse).ToArray())
    {
    }
}