using AutoMapper;
using dominikz.Api.Models;
using dominikz.Endpoints.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace dominikz.Api.Commands
{
    public class GetBlogpost : IRequest<VMBlogpost>
    {
        public int Id { get; }

        public GetBlogpost(int id)
        {
            Id = id;
        }
    }

    public class GetBlogpostHandler : IRequestHandler<GetBlogpost, VMBlogpost>
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public GetBlogpostHandler(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<VMBlogpost> Handle(GetBlogpost request, CancellationToken cancellationToken)
        {
            var post = await _context.Set<Blogpost>()
                .Include(x => x.Tags)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            return _mapper.Map<VMBlogpost>(post);
        }
    }
}
