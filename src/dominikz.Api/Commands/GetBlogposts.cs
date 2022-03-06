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
    public class GetBlogposts : IRequest<IReadOnlyList<VMBlogpostPreview>>
    {
    }

    public class GetBlogpostsHandler : IRequestHandler<GetBlogposts, IReadOnlyList<VMBlogpostPreview>>
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public GetBlogpostsHandler(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<IReadOnlyList<VMBlogpostPreview>> Handle(GetBlogposts request, CancellationToken cancellationToken)
        {
            var posts = await _context.Set<Blogpost>()
                .Include(x => x.Tags)
                .AsNoTracking()
                .Where(x => x.Release <= DateTime.Now)
                .OrderByDescending(x => x.Release)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<VMBlogpostPreview>>(posts);
        }
    }
}
