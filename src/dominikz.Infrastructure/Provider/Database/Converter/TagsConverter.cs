using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace dominikz.Infrastructure.Provider.Database.Converter;

internal class TagsConverter : ValueConverter<List<string>, string>
{
    public TagsConverter()
        : base(x => string.Join(';', x.Select(y => y.ToLower().Trim())),
            x => x.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList())
    {
    }
}