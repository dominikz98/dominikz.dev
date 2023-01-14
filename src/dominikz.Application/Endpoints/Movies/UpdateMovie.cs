using dominikz.Application.Utils;
using dominikz.Application.ViewModels;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Media;
using dominikz.Infrastructure.Mapper;
using dominikz.Infrastructure.Provider;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Application.Endpoints.Movies;

[Tags("movies")]
[Authorize(Policy = Policies.Media)]
[Authorize(Policy = Policies.CreateOrUpdate)]
[Route("api/movies")]
public class UpdateMovie : EndpointController
{
    private readonly IMediator _mediator;

    public UpdateMovie(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut]
    public async Task<IActionResult> Execute([FromForm] UpdateMovieRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        if (response.IsValid == false)
            return BadRequest(response.ToErrorList());

        return Ok(response.ViewModel);
    }
}

public class UpdateMovieRequest : FileUploadWrapper<EditMovieVm>, IRequest<ActionWrapper<MovieDetailVm>>
{
}

public class UpdateMovieRequestHandler : IRequestHandler<UpdateMovieRequest, ActionWrapper<MovieDetailVm>>
{
    private readonly DatabaseContext _database;
    private readonly IMediator _mediator;

    public UpdateMovieRequestHandler(DatabaseContext database, IMediator mediator)
    {
        _database = database;
        _mediator = mediator;
    }

    public async Task<ActionWrapper<MovieDetailVm>> Handle(UpdateMovieRequest request, CancellationToken cancellationToken)
    {
        // verify
        var expectedFilesCount = 1 + request.ViewModel.Directors.Count + request.ViewModel.Writers.Count + request.ViewModel.Stars.Count;
        if (request.Files.Count != expectedFilesCount)
            return new("Expected file count mismatch");

        // validate
        var toUpdate = await _database.From<Movie>().FirstOrDefaultAsync(x => x.Id == request.ViewModel.Id, cancellationToken);
        if (toUpdate == null)
            return new("Movie not found");

        toUpdate = toUpdate.ApplyChanges(request.ViewModel);
        _database.Update(toUpdate);
        await _database.SaveChangesAsync(cancellationToken);

        // load article detail
        var movie = await _mediator.Send(new GetMovieQuery(toUpdate.Id), cancellationToken);
        if (movie is null)
            return new("Error loading movie");

        return new ActionWrapper<MovieDetailVm>(movie);
    }
}