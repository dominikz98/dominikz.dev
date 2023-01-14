using dominikz.Application.Extensions;
using dominikz.Application.Utils;
using dominikz.Application.ViewModels;
using dominikz.Domain.Enums;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Blog;
using dominikz.Infrastructure.Mapper;
using dominikz.Infrastructure.Provider;
using dominikz.Infrastructure.Utils;
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
        var image = await UploadAndSaveImage(request, cancellationToken);
        if (image == null)
            return new("Invalid article image");
        
        // save article
        var toAdd = new Article().ApplyChanges(request.ViewModel, image.ContentType);
        var id = (await _database.AddAsync(toAdd, cancellationToken)).Entity.Id;
        
        // commit transactions
        await _storage.SaveChanges(cancellationToken);
        await _database.SaveChangesAsync(cancellationToken);

        // load article detail
        var article = await _mediator.Send(new GetArticleQuery(id), cancellationToken);
        if (article is null)
            return new("Error loading article");

        return new ActionWrapper<ArticleViewVm>(article);
    }

    private async Task<IFormFile?> UploadAndSaveImage(AddArticleRequest request, CancellationToken cancellationToken)
    {
        var file = request.Files.GetFileById(request.ViewModel.Id);
        if (file == null)
            return null;

        var image = file.OpenReadStream();
        image.Position = 0;
        
        var exists = await _storage.Exists(request.ViewModel.Id, cancellationToken);
        await _storage.Upload(request.ViewModel.Id, image, cancellationToken);
        if (exists)
            return file;
        
        await _database.AddAsync(new StorageFile()
        {
            Id = request.ViewModel.Id,
            Category = FileCategoryEnum.Image,
            Extension = FileIdentifier.GetExtensionByContentType(file.ContentType)
        }, cancellationToken);

        return file;
    }
}