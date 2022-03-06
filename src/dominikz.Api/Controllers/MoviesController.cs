using dominikz.Api.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace dominikz.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MoviesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetMovies(), cancellationToken));

        [HttpGet("search/info")]
        public async Task<IActionResult> GetAllCategories(CancellationToken cancellationToken)
           => Ok(await _mediator.Send(new GetAllMovieCategories(), cancellationToken));

        [HttpGet("featured/{count:int}")]
        public async Task<IActionResult> GetFeatured(int count, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetMovies() { Count = count, ListFeatured = true }, cancellationToken));

        [HttpGet("latest/{count:int}")]
        public async Task<IActionResult> GetLatest(int count, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetMovies() { Count = count }, cancellationToken));

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetMovie(id), cancellationToken));
    }
}
