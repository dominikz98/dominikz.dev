using dominikz.api.Extensions;
using dominikz.api.Provider;
using dominikz.api.Utils;
using dominikz.kernel.Endpoints;
using dominikz.kernel.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace dominikz.api.Commands;

public class SearchMediasQuery : MediaFilter, IRequest<IReadOnlyCollection<MediaVM>> { }

public class SearchMediaQueryHandler : IRequestHandler<SearchMediasQuery, IReadOnlyCollection<MediaVM>>
{
    private readonly ILinkCreator _linkCreator;
    private readonly DatabaseContext _database;

    public SearchMediaQueryHandler(ILinkCreator linkCreator, DatabaseContext database)
    {
        _linkCreator = linkCreator;
        _database = database;
    }

    public async Task<IReadOnlyCollection<MediaVM>> Handle(SearchMediasQuery request, CancellationToken cancellationToken)
    {
        var medias = await _database
            .Medias
            .Search(request)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // map viewmodels
        return medias.Select(x => new MediaVM
        {
            Id = x.Id,
            Title = x.Title,
            ImageUrl = _linkCreator.Create(x.FileId)!.ToString(),
            Timestamp = x.Timestamp,
            Rating = x.Rating,
            Category = x.Category,
            Genres = x.Genres.GetFlags()
        }).ToList();
    }
}