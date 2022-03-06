using AutoMapper;
using dominikz.Api.Models;
using dominikz.Endpoints.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace dominikz.Api.Commands
{
    public class GetPAP : IRequest<VMPenAndPaperAdventure>
    {
        public int Id { get; set; }

        public GetPAP(int id)
        {
            Id = id;
        }
    }

    public class GetPAPHanlder : IRequestHandler<GetPAP, VMPenAndPaperAdventure>
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public GetPAPHanlder(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<VMPenAndPaperAdventure> Handle(GetPAP request, CancellationToken cancellationToken)
        {
            var pap = await _context.Set<PenAndPaper>()
               .Include(x => x.Tags)
               .AsNoTracking()
               .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            return _mapper.Map<VMPenAndPaperAdventure>(pap);
        }
    }
}
