using dominikz.api.Provider;
using dominikz.kernel.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace dominikz.api.Commands;

public class GetFoodsQuery : IRequest<IReadOnlyCollection<FoodVM>>
{ }

public class GetFoodsQueryHandler : IRequestHandler<GetFoodsQuery, IReadOnlyCollection<FoodVM>>
{
    private readonly DatabaseContext _database;

    public GetFoodsQueryHandler(DatabaseContext database)
    {
        _database = database;
    }

    public async Task<IReadOnlyCollection<FoodVM>> Handle(GetFoodsQuery request, CancellationToken cancellationToken)
        => await _database.Foods
            .Select(x => new FoodVM()
            {
                Id = x.Id,
                Title = x.Title,
                PricePerCount = x.PricePerCount,
                Icon = x.Icon,
                Unit = x.Unit,
                Count = x.Count
            })
            .OrderBy(x => x.Title)
            .ToListAsync(cancellationToken);
}
