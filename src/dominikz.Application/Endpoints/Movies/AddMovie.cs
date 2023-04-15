using dominikz.Application.Extensions;
using dominikz.Application.Utils;
using dominikz.Application.ViewModels;
using dominikz.Domain.Enums;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Movies;
using dominikz.Infrastructure.Mapper;
using dominikz.Infrastructure.Provider.Database;
using dominikz.Infrastructure.Provider.Storage;
using dominikz.Infrastructure.Provider.Storage.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Application.Endpoints.Movies;

[Tags("movies")]
[Authorize(Policy = Policies.Movies)]
[Authorize(Policy = Policies.CreateOrUpdate)]
[Route("api/movies")]
public class AddMovie : EndpointController
{
    private readonly IMediator _mediator;

    public AddMovie(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Execute([FromForm] AddMovieRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        if (response.IsValid == false)
            return BadRequest(response.ToErrorList());

        return Ok(response.ViewModel);
    }
}

public class AddMovieRequest : FileUploadWrapper<EditMovieVm>, IRequest<ActionWrapper<MovieDetailVm>>
{
}

public class AddMovieRequestHandler : IRequestHandler<AddMovieRequest, ActionWrapper<MovieDetailVm>>
{
    private readonly DatabaseContext _database;
    private readonly IStorageProvider _storage;
    private readonly IMediator _mediator;

    public AddMovieRequestHandler(DatabaseContext database, IStorageProvider storage, IMediator mediator)
    {
        _database = database;
        _storage = storage;
        _mediator = mediator;
    }

    public async Task<ActionWrapper<MovieDetailVm>> Handle(AddMovieRequest request, CancellationToken cancellationToken)
    {
        // validate
        var alreadyExists = await _database.From<Movie>()
            .AnyAsync(x => EF.Functions.Like(x.Title, request.ViewModel.Title)
                           || x.Id == request.ViewModel.Id
                           || x.ImdbId == request.ViewModel.ImdbId, cancellationToken);

        if (alreadyExists)
            return new("Movie already exists");

        // upload poster
        var file = request.Files.GetBySingleOrId(request.ViewModel.Id);
        if (file == null)
            return new("Invalid movie poster");

        var stream = file.OpenReadStream();
        stream.Position = 0;
        await _storage.Upload(new UploadImageRequest(request.ViewModel.Id, stream), cancellationToken);
        await _storage.Upload(new UploadImageRequest(request.ViewModel.Id, stream, ImageSizeEnum.ThumbnailVertical), cancellationToken);

        // save movie
        var toAdd = new Movie().ApplyChanges(request.ViewModel);
        await _database.AddAsync(toAdd, cancellationToken);
        await _database.SaveChangesAsync(cancellationToken);

        // load article detail
        var movie = await _mediator.Send(new GetMovieQuery(toAdd.Id), cancellationToken);
        if (movie is null)
            return new("Error loading movie");

        return new ActionWrapper<MovieDetailVm>(movie);
    }
}