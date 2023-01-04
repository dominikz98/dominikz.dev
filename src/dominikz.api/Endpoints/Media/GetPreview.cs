using dominikz.api.Mapper;
using dominikz.api.Provider;
using dominikz.api.Utils;
using dominikz.shared.Contracts;
using dominikz.shared.ViewModels.Media;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.api.Endpoints.Media;

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

public record GetPreviewQuery : IRequest<IReadOnlyCollection<MediaVM>>;

public class GetFoodsQueryHandler : IRequestHandler<GetPreviewQuery, IReadOnlyCollection<MediaVM>>
{
    private readonly DatabaseContext _database;
    private readonly ILinkCreator _linkCreator;

    public GetFoodsQueryHandler(DatabaseContext database, ILinkCreator linkCreator)
    {
        _database = database;
        _linkCreator = linkCreator;
    }

    public async Task<IReadOnlyCollection<MediaVM>> Handle(GetPreviewQuery request, CancellationToken cancellationToken)
    {
        var previews = new List<MediaPreviewVM>();

        foreach (var category in Enum.GetValues<MediaCategoryEnum>())
        {
            var previewsByCategory = await GetNewestPreviews(category, cancellationToken);

            foreach (var preview in previewsByCategory)
                if (preview.ImageUrl != string.Empty)
                    preview.ImageUrl = _linkCreator.CreateImageUrl(preview.ImageUrl, ImageSizeEnum.Carousel);

            previews.AddRange(previewsByCategory);
        }

        return previews.OrderByDescending(x => x.Timestamp).ToList();
    }

    private async Task<IReadOnlyCollection<MediaPreviewVM>> GetNewestPreviews(MediaCategoryEnum category, CancellationToken cancellationToken)
        => await _database.From<Models.Media>()
            .Include(x => x.File)
            .AsNoTracking()
            .Where(x => x.Category == category)
            .OrderByDescending(x => x.Timestamp)
            .Take(3)
            .MapToPreviewVm()
            .ToListAsync(cancellationToken);
}