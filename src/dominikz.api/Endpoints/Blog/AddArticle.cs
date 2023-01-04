using dominikz.api.Mapper;
using dominikz.api.Models;
using dominikz.api.Models.ViewModels;
using dominikz.api.Provider;
using dominikz.api.Utils;
using dominikz.shared.ViewModels.Blog;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace dominikz.api.Endpoints.Blog;

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
    public async Task<IActionResult> Execute([FromForm] FileUploadWrapper<AddArticleVm> request, CancellationToken cancellationToken)
    {
        var image = request.Files.First();
        var vm = request.ViewModel.MapToModel(image.ContentType);

        var response = await _mediator.Send(new AddArticleRequest(vm, image), cancellationToken);
        if (response.IsValid == false)
            return BadRequest(response.ToErrorList());

        return Ok(response.ViewModel);
    }
}

public record AddArticleRequest(Article Article, IFormFile Image) : IRequest<ActionWrapper<ArticleViewVm>>;

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
        // validate
        var alreadyExists = await _database.From<Article>().AnyAsync(x => EF.Functions.Like(x.Title, request.Article.Title), cancellationToken);
        if (alreadyExists)
            return new("Article already exists");

        // upload file
        var image = request.Image.OpenReadStream();
        image.Position = 0;
        await _storage.Upload(request.Article.FileId, image, cancellationToken);

        // save article
        var id = (await _database.AddAsync(request.Article, cancellationToken)).Entity.Id;
        await _database.SaveChangesAsync(cancellationToken);

        // load article detail
        var article = await _mediator.Send(new GetArticleQuery(id), cancellationToken);
        if (article is null)
            return new("Error loading article");

        return new ActionWrapper<ArticleViewVm>(article);
    }
}