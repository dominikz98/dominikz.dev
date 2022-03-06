using dominikz.Api.Commands;
using dominikz.Common.Enumerations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace dominikz.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScriptsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ScriptsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("csharp")]
        public async Task<IActionResult> GetCSharp(CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetScripts(ScriptType.CSharp), cancellationToken));

        [HttpGet("sql")]
        public async Task<IActionResult> GetSQL(CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetScripts(ScriptType.SQL), cancellationToken));
    }
}
