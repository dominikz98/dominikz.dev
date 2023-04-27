using dominikz.Api.Attributes;
using dominikz.Infrastructure.Clients.Finance;
using dominikz.Infrastructure.Provider.Database;
using dominikz.Infrastructure.Worker;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace dominikz.Api.Endpoints.Admin;

[Tags("admin")]
[ApiKey(RequiresMasterKey = true)]
[ApiController]
[Route("api/admin/playground")]
public class Playground : ControllerBase
{
    private readonly FinnhubClient _finnhubClient;
    private readonly OnVistaClient _onVistaClient;
    private readonly DatabaseContext _context;
    private readonly ILogger<FinnhubMirror> _logger;

    public Playground(FinnhubClient finnhubClient, OnVistaClient onVistaClient, DatabaseContext context, ILogger<FinnhubMirror> logger)
    {
        _finnhubClient = finnhubClient;
        _onVistaClient = onVistaClient;
        _context = context;
        _logger = logger;
    }
    
    [HttpPost]
    public async Task<IActionResult> Execute()
    {
        await new FinnhubMirror(_finnhubClient, _onVistaClient, _context, _logger).Execute(default);
        return Ok();
    }
}