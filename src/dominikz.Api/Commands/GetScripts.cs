using AutoMapper;
using dominikz.Api.Models;
using dominikz.Common.Enumerations;
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
    public class GetScripts : IRequest<IReadOnlyList<VMScript>>
    {
        public ScriptType Type { get; }

        public GetScripts(ScriptType type)
        {
            Type = type;
        }
    }

    public class GetScriptsHandler : IRequestHandler<GetScripts, IReadOnlyList<VMScript>>
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public GetScriptsHandler(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<IReadOnlyList<VMScript>> Handle(GetScripts request, CancellationToken cancellationToken)
        {
            var scripts = await _context.Set<Script>()
                .Include(x => x.Tags)
                .AsNoTracking()
                .Where(x => x.Release <= DateTime.Now)
                .Where(x => x.Type == request.Type)
                .OrderByDescending(x => x.Release)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<VMScript>>(scripts);
        }
    }
}
