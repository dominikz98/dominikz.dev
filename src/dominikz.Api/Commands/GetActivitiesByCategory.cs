using AutoMapper;
using dominikz.Api.Models;
using dominikz.Common.Enumerations;
using dominikz.Endpoints.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace dominikz.Api.Commands
{
    public class GetActivitiesByCategory : IRequest<IReadOnlyList<VMActivity>>
    {
        public ActivityCategory Category { get; }

        public GetActivitiesByCategory(ActivityCategory category)
        {
            Category = category;
        }
    }

    public class GetActivitiesByCategoryHandler : IRequestHandler<GetActivitiesByCategory, IReadOnlyList<VMActivity>>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public GetActivitiesByCategoryHandler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<VMActivity>> Handle(GetActivitiesByCategory request, CancellationToken cancellationToken)
        {
            var activities = await _context.Set<Activity>()
                .Include(x => x.Tags)
                .AsNoTracking()
                .Where(x => request.Category == ActivityCategory.All || x.Category == request.Category)
                .OrderByDescending(x => x.Release)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<VMActivity>>(activities);
        }
    }
}
