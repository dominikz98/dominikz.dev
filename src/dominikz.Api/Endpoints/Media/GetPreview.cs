using dominikz.api.Mapper;
using dominikz.api.Provider;
using dominikz.api.Utils;
using dominikz.kernel.Contracts;
using dominikz.kernel.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.api.Endpoints.Media;

[Tags("medias")]
[ApiController]
[Route("api/medias")]
public class GetPreview : ControllerBase
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

public class GetPreviewQuery : IRequest<IReadOnlyCollection<MediaVM>> { }

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

        foreach (var category in Enum.GetValues<MediaCategoryEnum>()[1..])
        {
            var preview = await GetNewestPreview(category, cancellationToken);

            if (preview is null)
                continue;

            if (preview.Image is not null)
                preview.Image.Url = _linkCreator.CreateImageUrl(preview.Image.Id, ImageSizeEnum.Carousel)?.ToString() ?? string.Empty;

            previews.Add(preview);
        }

        return previews;
    }

    private async Task<MediaPreviewVM?> GetNewestPreview(MediaCategoryEnum category, CancellationToken cancellationToken)
        => await _database.From<Models.Media>()
            .Include(x => x.File)
            .AsNoTracking()
            .Where(x => x.Category == category)
            .OrderByDescending(x => x.Timestamp)
            .MapToPreviewVM()
            .FirstOrDefaultAsync(cancellationToken);
}
