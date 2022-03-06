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
    public class GetPAPs : IRequest<IReadOnlyList<VMPenAndPaper>>
    {
    }

    public class GetPAPsHandler : IRequestHandler<GetPAPs, IReadOnlyList<VMPenAndPaper>>
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public GetPAPsHandler(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<IReadOnlyList<VMPenAndPaper>> Handle(GetPAPs request, CancellationToken cancellationToken)
        {
            var paps = await _context.Set<PenAndPaper>()
               .AsNoTracking()
               .Where(x => x.Release <= DateTime.Now)
               .OrderByDescending(x => x.Release)
               .ToListAsync(cancellationToken);

            return _mapper.Map<List<VMPenAndPaper>>(paps);
        }
    }
}
