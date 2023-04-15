using dominikz.Application.Utils;
using dominikz.Domain.ViewModels.Cookbook;
using dominikz.Infrastructure.Clients.SupermarktCheck;
using dominikz.Infrastructure.Mapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dominikz.Application.Endpoints.Cookbook;

[Tags("cookbook")]
[Route("api/cookbook/foods/template")]
[ResponseCache(Duration = 86400)]
[Authorize(Policy = Policies.Cookbook)]
[Authorize(Policy = Policies.CreateOrUpdate)]
public class GetFoodTemplate : EndpointController
{
    private readonly IMediator _mediator;

    public GetFoodTemplate(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Execute(int id, CancellationToken cancellationToken)
    {
        var vm = await _mediator.Send(new GetFoodTemplateQuery(id), cancellationToken);
        if (vm == null)
            return NotFound();

        return Ok(vm);
    }
}

public class GetFoodTemplateQuery : IRequest<FoodVm?>
{
    public int Id { get; }

    public GetFoodTemplateQuery(int id)
    {
        Id = id;
    }
}

public class GetFoodTemplateQueryHandler : IRequestHandler<GetFoodTemplateQuery, FoodVm?>
{
    private readonly SupermarktCheckClient _client;

    public GetFoodTemplateQueryHandler(SupermarktCheckClient client)
    {
        _client = client;
    }

    public async Task<FoodVm?> Handle(GetFoodTemplateQuery request, CancellationToken cancellationToken)
    {
        var product = await _client.GetProductById(request.Id, cancellationToken);
        if (product == null)
            return null;

        return product.MapToDetailVm();
    }
}