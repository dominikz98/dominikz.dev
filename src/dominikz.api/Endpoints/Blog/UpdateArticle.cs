using dominikz.api.Mapper;
using dominikz.api.Models;
using dominikz.api.Models.ViewModels;
using dominikz.api.Provider;
using dominikz.api.Utils;
using dominikz.shared.ViewModels.Blog;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.api.Endpoints.Blog;

[Tags("blog")]
[Authorize(Policy = Policies.Blog)]
[Authorize(Policy = Policies.CreateOrUpdate)]
[Route("api/blog")]
public class UpdateArticle : EndpointController
{
    private readonly IMediator _mediator;

    public UpdateArticle(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut]
    public async Task<IActionResult> Execute([FromForm] FileUploadWrapper<EditArticleVm> request, CancellationToken cancellationToken)
    {
        var image = request.Files.First();
        var vm = request.ViewModel.MapToModel(image.ContentType);

        var response = await _mediator.Send(new EditArticleRequest(vm, image), cancellationToken);
        if (response.IsValid == false)
            return BadRequest(response.ToErrorList());

        return Ok(response.ViewModel);
    }
}

public record EditArticleRequest(Article Article, IFormFile Image) : IRequest<ActionWrapper<ArticleViewVm>>;

public class EditArticleRequestHandler : IRequestHandler<EditArticleRequest, ActionWrapper<ArticleViewVm>>
{
    private readonly DatabaseContext _database;
    private readonly IStorageProvider _storage;
    private readonly IMediator _mediator;

    public EditArticleRequestHandler(DatabaseContext database, IStorageProvider storage, IMediator mediator)
    {
        _database = database;
        _storage = storage;
        _mediator = mediator;
    }

    public async Task<ActionWrapper<ArticleViewVm>> Handle(EditArticleRequest request, CancellationToken cancellationToken)
    {
        // validate
        var original = await _database.From<Article>()
            .Include(x => x.File)
            .FirstOrDefaultAsync(x => x.Id == request.Article.Id, cancellationToken);

        if (original == null)
            return new ActionWrapper<ArticleViewVm>("Article not exists");

        await RefreshImageIfRequired(original, request, cancellationToken);

        // apply changes
        original.ApplyChanges(request.Article, request.Image.ContentType);
        _database.Update(original);
        await _database.SaveChangesAsync(cancellationToken);

        // load article detail
        var article = await _mediator.Send(new GetArticleQuery(original.Id), cancellationToken);
        if (article is null)
            return new ActionWrapper<ArticleViewVm>("Error loading article");

        return new ActionWrapper<ArticleViewVm>(article);
    }

    private async Task RefreshImageIfRequired(Article original, EditArticleRequest request, CancellationToken cancellationToken)
    {
        // check for file changed
        var originalImg = await _storage.Download(original.FileId, cancellationToken);
        var currentImg = request.Image.OpenReadStream();
        if (originalImg == null)
        {
            await _storage.Upload(request.Article.FileId, currentImg, cancellationToken);
            return;
        }

        // compare
        if (originalImg.Length == currentImg.Length)
            return;

        // override old image
        await _storage.Delete(original.FileId, cancellationToken);
        currentImg.Position = 0;
        await _storage.Upload(request.Article.FileId, currentImg, cancellationToken);
    }
}