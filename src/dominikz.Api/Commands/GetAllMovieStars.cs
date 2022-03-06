using AutoMapper;
using dominikz.Api.Models;
using dominikz.Endpoints.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace dominikz.Api.Commands
{
    public class GetAllMovieStars : IRequest<IReadOnlyList<VMMovieStar>>
    {
    }

    public class GetAllMovieStarsHandler : IRequestHandler<GetAllMovieStars, IReadOnlyList<VMMovieStar>>
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public GetAllMovieStarsHandler(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<IReadOnlyList<VMMovieStar>> Handle(GetAllMovieStars request, CancellationToken cancellationToken)
        {
            var stars = await _context.Set<MovieStar>()
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<VMMovieStar>>(stars);
        }
    }
}
