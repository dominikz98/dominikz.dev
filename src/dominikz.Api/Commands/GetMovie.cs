using AutoMapper;
using dominikz.Api.Models;
using dominikz.Endpoints.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace dominikz.Api.Commands
{
    public class GetMovie : IRequest<VMMovie>
    {
        public int Id { get; set; }

        public GetMovie(int id)
        {
            Id = id;
        }
    }

    public class GetMovieHandler : IRequestHandler<GetMovie, VMMovie>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public GetMovieHandler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<VMMovie> Handle(GetMovie request, CancellationToken cancellationToken)
        {
            var movie = await _context.Set<Movie>()
                .Include(x => x.Tags)
                .Include(x => x.Stars)
                .Include(x => x.Rating)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            return _mapper.Map<VMMovie>(movie);
        }
    }
}
