using dominikz.Api.Attributes;
using dominikz.Api.Background;
using dominikz.Domain.Models;
using dominikz.Infrastructure.Clients.Finance;
using dominikz.Infrastructure.Provider.Database;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace dominikz.Api.Endpoints.Admin;

[Tags("admin")]
[ApiKey(RequiresMasterKey = true)]
[ApiController]
[Route("api/admin/playground")]
public class Playground : ControllerBase
{
    private readonly OnVistaClient _onVista;
    private readonly EarningsWhispersClient _whispers;
    private readonly FinnhubClient _finnhub;
    private readonly DatabaseContext _database;

    public Playground(FinnhubClient finnhub, OnVistaClient onVista, EarningsWhispersClient whispers, DatabaseContext database)
    {
        _onVista = onVista;
        _whispers = whispers;
        _finnhub = finnhub;
        _database = database;
    }

    [HttpPost]
    public async Task<IActionResult> Execute(CancellationToken cancellationToken)
    {
        // var whispersLog = new WorkerLog()
        // {
        //     Worker = nameof(WhispersMirror),
        //     Timestamp = DateTime.Now
        // };
        // await new WhispersMirror(_whispers, _database).Execute(whispersLog, cancellationToken);

        var finnhubLog = new WorkerLog()
        {
            Worker = nameof(FinnhubMirror),
            Timestamp = DateTime.Now
        };
        await new FinnhubMirror(_finnhub, _onVista, _database).Execute(finnhubLog, cancellationToken);
        return Ok(new[] { finnhubLog });
    }
}