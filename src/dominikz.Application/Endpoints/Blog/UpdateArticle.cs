using dominikz.Application.Utils;
using dominikz.Application.ViewModels;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Blog;
using dominikz.Infrastructure.Mapper;
using dominikz.Infrastructure.Provider.Database;
using dominikz.Infrastructure.Provider.Storage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Application.Endpoints.Blog;

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
        var response = await _mediator.Send((EditArticleRequest)request, cancellationToken);
        if (response.IsValid == false)
            return BadRequest(response.ToErrorList());

        return Ok(response.ViewModel);
    }
}

public class EditArticleRequest : FileUploadWrapper<EditArticleVm>, IRequest<ActionWrapper<ArticleViewVm>>
{
}

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
        // verify
        if (request.Files.Count != 1)
            return new("Expected file count mismatch");

        // validate
        var original = await _database.From<Article>().FirstOrDefaultAsync(x => x.Id == request.ViewModel.Id, cancellationToken);

        if (original == null)
            return new ActionWrapper<ArticleViewVm>("Article not found");

        // upload file
        var file = request.Files.First();
        var image = file.OpenReadStream();
        image.Position = 0;
        await _storage.TryDelete(new DeleteImageRequest(request.ViewModel.Id), cancellationToken);
        await _storage.Upload(new UploadImageRequest(request.ViewModel.Id, image), cancellationToken);

        // apply changes
        original.ApplyChanges(request.ViewModel);
        _database.Update(original);

        // commit transactions
        await _database.SaveChangesAsync(cancellationToken);

        // load article detail
        var article = await _mediator.Send(new GetArticleQuery(original.Id), cancellationToken);
        if (article is null)
            return new ActionWrapper<ArticleViewVm>("Error loading article");

        return new ActionWrapper<ArticleViewVm>(article);
    }
}