using AutoMapper;
using dominikz.Api.Models;
using dominikz.Endpoints.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace dominikz.Api.Commands
{
    public class GetMovies : IRequest<IReadOnlyList<VMMoviePreview>>
    {
        public int? Count { get; set; }
        public bool ListFeatured { get; set; }
    }

    public class GetMoviesHandler : IRequestHandler<GetMovies, IReadOnlyList<VMMoviePreview>>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public GetMoviesHandler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<VMMoviePreview>> Handle(GetMovies request, CancellationToken cancellationToken)
        {
            var query = _context.Set<Movie>()
                .Include(x => x.Tags)
                .Include(x => x.Rating)
                .AsNoTracking()
                .Where(x => x.Release <= DateTime.Now);

            if (request.Count is not null)
                query = query.Take(request.Count.Value);

            if (!request.ListFeatured)
            {
                var movies = await query.OrderByDescending(x => x.Release)
                    .ToListAsync(cancellationToken);

                return _mapper.Map<List<VMMoviePreview>>(movies);
            }

            var featuredMovies = await query.Select(x => new
            {
                Movie = x,
                Rating = (x.Rating.Actors + x.Rating.Ambience + x.Rating.Music + x.Rating.Plot + x.Rating.Regie) / 5.0
            })
            .OrderByDescending(x => x.Rating)
            .Select(x => x.Movie)
            .ToListAsync(cancellationToken);

            return _mapper.Map<List<VMMoviePreview>>(featuredMovies);
        }
    }
}
