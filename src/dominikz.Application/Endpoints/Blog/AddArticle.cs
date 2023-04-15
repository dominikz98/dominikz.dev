using dominikz.Application.Extensions;
using dominikz.Application.Utils;
using dominikz.Application.ViewModels;
using dominikz.Domain.Enums;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Blog;
using dominikz.Infrastructure.Mapper;
using dominikz.Infrastructure.Provider.Database;
using dominikz.Infrastructure.Provider.Storage;
using dominikz.Infrastructure.Provider.Storage.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Application.Endpoints.Blog;

[Tags("blog")]
[Authorize(Policy = Policies.Blog)]
[Authorize(Policy = Policies.CreateOrUpdate)]
[Route("api/blog")]
public class AddArticle : EndpointController
{
    private readonly IMediator _mediator;

    public AddArticle(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Execute([FromForm] FileUploadWrapper<EditArticleVm> request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send((AddArticleRequest)request, cancellationToken);
        if (response.IsValid == false)
            return BadRequest(response.ToErrorList());

        return Ok(response.ViewModel);
    }
}

public class AddArticleRequest : FileUploadWrapper<EditArticleVm>, IRequest<ActionWrapper<ArticleViewVm>>
{
}

public class AddArticleRequestHandler : IRequestHandler<AddArticleRequest, ActionWrapper<ArticleViewVm>>
{
    private readonly DatabaseContext _database;
    private readonly IStorageProvider _storage;
    private readonly IMediator _mediator;

    public AddArticleRequestHandler(DatabaseContext database, IStorageProvider storage, IMediator mediator)
    {
        _database = database;
        _storage = storage;
        _mediator = mediator;
    }

    public async Task<ActionWrapper<ArticleViewVm>> Handle(AddArticleRequest request, CancellationToken cancellationToken)
    {
        // verify
        if (request.Files.Count != 1)
            return new("Expected file count mismatch");

        // validate
        var alreadyExists = await _database.From<Article>()
            .AnyAsync(x => EF.Functions.Like(x.Title, request.ViewModel.Title)
                           || x.Id == request.ViewModel.Id, cancellationToken);

        if (alreadyExists)
            return new("Article already exists");

        // upload file
        var file = request.Files.GetBySingleOrId(request.ViewModel.Id);
        if (file == null)
            return new("Invalid article image");
        
        var stream = file.OpenReadStream();
        stream.Position = 0;
        await _storage.Upload(new UploadImageRequest(request.ViewModel.Id, stream), cancellationToken);
        await _storage.Upload(new UploadImageRequest(request.ViewModel.Id, stream, ImageSizeEnum.ThumbnailHorizontal), cancellationToken);

        // save article
        var toAdd = new Article().ApplyChanges(request.ViewModel);
        var id = (await _database.AddAsync(toAdd, cancellationToken)).Entity.Id;

        // commit transactions
        await _database.SaveChangesAsync(cancellationToken);

        // load article detail
        var article = await _mediator.Send(new GetArticleQuery(id), cancellationToken);
        if (article is null)
            return new("Error loading article");

        return new ActionWrapper<ArticleViewVm>(article);
    }
}