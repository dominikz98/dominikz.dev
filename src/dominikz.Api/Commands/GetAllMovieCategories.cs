using dominikz.Api.Models;
using dominikz.Common.Enumerations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace dominikz.Api.Commands
{
    public class GetAllMovieCategories : IRequest<IReadOnlyList<string>>
    {
    }

    public class GetAllMovieCategoriesHandler : IRequestHandler<GetAllMovieCategories, IReadOnlyList<string>>
    {
        private readonly DataContext _context;

        public GetAllMovieCategoriesHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<string>> Handle(GetAllMovieCategories request, CancellationToken cancellationToken)
            => await _context.Set<ItemTag>()
                .Where(x => x.Type == TagType.MovieCategory)
                .Select(x => x.Name)
                .ToListAsync(cancellationToken);
    }
}
