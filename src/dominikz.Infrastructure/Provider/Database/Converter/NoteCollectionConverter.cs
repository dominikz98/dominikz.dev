using dominikz.Domain.Structs;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace dominikz.Infrastructure.Provider.Database.Converter;

internal class NoteCollectionConverter : ValueConverter<NoteCollection, string>
{
    public NoteCollectionConverter()
        : base(x => x.ToString(),
            x => new NoteCollection(x))
    {
    }
}