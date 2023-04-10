using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace dominikz.Infrastructure.Provider.Database.Converter;

internal class ListComparer<T> : ValueComparer<List<T>>
{
    public ListComparer()
        : base((c1, c2) => (c1 ?? new()).SequenceEqual(c2 ?? new()),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v!.GetHashCode())),
            c => c.ToList())
    {
    }
}