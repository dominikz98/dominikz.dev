using dominikz.Application.Utils;
using dominikz.Domain.Enums;
using dominikz.Domain.Enums.Media;
using dominikz.Domain.ViewModels.Media;
using dominikz.Infrastructure.Mapper;
using dominikz.Infrastructure.Provider;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Application.Endpoints.Media;

[Tags("medias")]
[Route("api/medias")]
public class GetPreview : EndpointController
{
    private readonly IMediator _mediator;

    public GetPreview(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("preview")]
    public async Task<IActionResult> Execute(CancellationToken cancellationToken)
    {
        var vms = await _mediator.Send(new GetPreviewQuery(), cancellationToken);
        return Ok(vms);
    }
}

public record GetPreviewQuery : IRequest<IReadOnlyCollection<MediaVm>>;

public class GetFoodsQueryHandler : IRequestHandler<GetPreviewQuery, IReadOnlyCollection<MediaVm>>
{
    private readonly DatabaseContext _database;
    private readonly ILinkCreator _linkCreator;
    private readonly CredentialsProvider _credentials;

    public GetFoodsQueryHandler(DatabaseContext database, ILinkCreator linkCreator, CredentialsProvider credentials)
    {
        _database = database;
        _linkCreator = linkCreator;
        _credentials = credentials;
    }

    public async Task<IReadOnlyCollection<MediaVm>> Handle(GetPreviewQuery request, CancellationToken cancellationToken)
    {
        var previews = new List<MediaPreviewVm>();

        foreach (var category in Enum.GetValues<MediaCategoryEnum>())
        {
            var previewsByCategory = await GetNewestPreviews(category, cancellationToken);

            foreach (var preview in previewsByCategory)
                if (preview.ImageUrl != string.Empty)
                    preview.ImageUrl = _linkCreator.CreateImageUrl(preview.ImageUrl, ImageSizeEnum.Carousel);

            previews.AddRange(previewsByCategory);
        }

        return previews.OrderByDescending(x => x.PublishDate).ToList();
    }

    private async Task<IReadOnlyCollection<MediaPreviewVm>> GetNewestPreviews(MediaCategoryEnum category, CancellationToken cancellationToken)
    {
        var query = _database.From<dominikz.Domain.Models.Media>()
            .Include(x => x.File)
            .AsNoTracking()
            .Where(x => x.Category == category);

        if (_credentials.HasPermission(PermissionFlags.Media) == false)
            query = query.Where(x => x.PublishDate != null);
        
        return await query
            .OrderByDescending(x => x.PublishDate)
            .Take(3)
            .MapToPreviewVm()
            .ToListAsync(cancellationToken);
    }
}