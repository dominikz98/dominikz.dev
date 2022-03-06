using dominikz.Api.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace dominikz.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PAPController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PAPController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet()]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
          => Ok(await _mediator.Send(new GetPAPs(), cancellationToken));

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
          => Ok(await _mediator.Send(new GetPAP(id), cancellationToken));
    }
}
