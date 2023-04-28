using System.Diagnostics.CodeAnalysis;
using dominikz.Api.Attributes;
using dominikz.Domain.Models;
using dominikz.Infrastructure.Clients;
using dominikz.Infrastructure.Clients.Finance;
using dominikz.Infrastructure.Provider.Database;
using dominikz.Infrastructure.Worker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    private readonly EarningsWhispersClient _whispersClient;
    private readonly EmailClient _emailClient;
    private readonly ILogger<WhispersMirror> _whispersLogger;
    private readonly ILogger<FinnhubMirror> _finnhubLogger;

    [SuppressMessage("ReSharper", "ContextualLoggerProblem")]
    public Playground(FinnhubClient finnhubClient,
        OnVistaClient onVistaClient,
        DatabaseContext context,
        EarningsWhispersClient whispersClient,
        EmailClient emailClient,
        ILogger<WhispersMirror> whispersLogger,
        ILogger<FinnhubMirror> finnhubLogger)
    {
        _finnhubClient = finnhubClient;
        _onVistaClient = onVistaClient;
        _context = context;
        _whispersClient = whispersClient;
        _emailClient = emailClient;
        _whispersLogger = whispersLogger;
        _finnhubLogger = finnhubLogger;
    }

    [HttpPost("whispers")]
    public async Task<IActionResult> ExecuteWhispersMirror(CancellationToken cancellationToken)
    {
        await new WhispersMirror(_whispersClient, _context, _whispersLogger).Execute(cancellationToken);
        return Ok();
    }

    [HttpPost("finnhub")]
    public async Task<IActionResult> ExecuteFinnhubMirror()
    {
        await new FinnhubMirror(_finnhubClient, _onVistaClient, _context, _emailClient, _finnhubLogger).Execute(default);
        return Ok();
    }
}