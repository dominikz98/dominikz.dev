// using dominikz.Domain.Enums.Trades;
// using dominikz.Domain.Models;
// using dominikz.Domain.Structs;
// using dominikz.Infrastructure.Clients.Finance;
// using dominikz.Infrastructure.Provider.Database;
// using dominikz.Infrastructure.Provider.Storage;
// using dominikz.Infrastructure.Provider.Storage.Requests;
// using ImageMagick;
// using Microsoft.EntityFrameworkCore;
//
// namespace dominikz.Api.Background;
//
// public class EarningsCallsRefresher : ITimeTriggeredWorker
// {
//     // At minute 0 past every hour from 6 through 16 on every day-of-week from Monday through Friday.
//     public CronSchedule Schedule { get; } = new("0 6-23 * * 1-5");
//
//     private readonly EarningsWhispersClient _eaClient;
//     private readonly OnVistaClient _onVistaClient;
//     private readonly AktienFinderClient _aktienFinderClient;
//     private readonly FinanzenNetClient _finanzenNetClient;
//     private readonly FinnhubClient _finnhubClient;
//     private readonly IStorageProvider _storage;
//     private readonly DatabaseContext _database;
//
//     public EarningsCallsRefresher(EarningsWhispersClient eaClient,
//         OnVistaClient onVistaClient,
//         AktienFinderClient aktienFinderClient,
//         FinanzenNetClient finanzenNetClient,
//         FinnhubClient finnhubClient,
//         IStorageProvider storage,
//         DatabaseContext database)
//     {
//         _eaClient = eaClient;
//         _onVistaClient = onVistaClient;
//         _aktienFinderClient = aktienFinderClient;
//         _finanzenNetClient = finanzenNetClient;
//         _finnhubClient = finnhubClient;
//         _storage = storage;
//         _database = database;
//     }
//
//     public async Task<bool> Execute(WorkerLog log, CancellationToken cancellationToken)
//     {
//         var ewCalls = await _eaClient.GetEarningsCallsOfToday();
//
//         var addCounter = 0;
//         var updCounter = 0;
//         foreach (var ewCall in ewCalls)
//         {
//             // update
//             var updated = await UpdateCallIfRequired(ewCall, cancellationToken);
//             if (updated != null)
//             {
//                 updCounter++;
//                 continue;
//             }
//
//             // add
//             await AddNewCall(ewCall, cancellationToken);
//             addCounter++;
//         }
//
//         log.Log += $"{addCounter} earnings calls added. {updCounter} earnings calls updated";
//         return true;
//     }
//
//     private async Task AddNewCall(EwCall ewCall, CancellationToken cancellationToken)
//     {
//         var call = new EarningCall
//         {
//             Date = DateOnly.FromDateTime(DateTime.Now),
//             Sources = InformationSource.EarningsWhispers,
//             Symbol = ewCall.Symbol,
//             Name = ewCall.Name,
//             Release = ewCall.Release,
//             Growth = ewCall.Growth,
//             Surprise = ewCall.Surprise,
//             Updated = DateTime.UtcNow
//         };
//
//         var stock = await GetStockBySymbolAndName(call, cancellationToken);
//         if (stock != null)
//         {
//             call.ISIN = stock.ISIN;
//             call.OnVistaLink = stock.Urls.Website;
//             call.OnVistaNewsLink = stock.Urls.News;
//             await DownloadLogo(call, cancellationToken);
//         }
//
//         await _database.AddAsync(call, cancellationToken);
//         await _database.SaveChangesAsync(cancellationToken);
//     }
//
//     private async Task<EarningCall?> UpdateCallIfRequired(EwCall ewCall, CancellationToken cancellationToken)
//     {
//         var date = DateOnly.FromDateTime(DateTime.Now);
//         var call = await _database.From<EarningCall>()
//             .Where(x => x.Date == date)
//             .Where(x => x.Symbol == ewCall.Symbol)
//             .FirstOrDefaultAsync(cancellationToken);
//
//         if (call == null)
//             return null;
//
//         call.Surprise = ewCall.Surprise;
//         call.Growth = ewCall.Growth;
//         call.Updated = DateTime.UtcNow;
//         _database.Update(call);
//         await _database.SaveChangesAsync(cancellationToken);
//         return null;
//     }
//
//     private async Task DownloadLogo(EarningCall call, CancellationToken cancellationToken)
//     {
//         string? logoUrl = null;
//         InformationSource? source = null;
//
//         if (string.IsNullOrWhiteSpace(call.ISIN) == false)
//             logoUrl = (await _finnhubClient.TryGetCompanyByISIN(call.ISIN, cancellationToken))?.Logo;
//
//         logoUrl ??= (await _finnhubClient.TryGetCompanyBySymbol(call.Symbol, cancellationToken))?.Logo;
//         if (string.IsNullOrWhiteSpace(logoUrl) == false)
//             source = InformationSource.Finnhub;
//
//         logoUrl ??= await _aktienFinderClient.GetLogoByISIN(call.ISIN!);
//         if (string.IsNullOrWhiteSpace(logoUrl))
//             return;
//
//         source = InformationSource.AktienFinder;
//         var data = await new HttpClient().GetStreamAsync(logoUrl, cancellationToken);
//         var format = MagickFormatInfo.Create(logoUrl)?.Format ?? MagickFormat.Jpg;
//         await _storage.Upload(new UploadLogoRequest(call.Symbol, data, format), cancellationToken);
//
//         call.Sources |= source.Value;
//         call.HasLogo = true;
//     }
//
//     private async Task<OvResult?> GetStockBySymbolAndName(EarningCall call, CancellationToken cancellationToken)
//     {
//         var stock = await _onVistaClient.GetStockBySymbolAndName(call.Symbol, call.Name, cancellationToken);
//         if (stock != null)
//         {
//             call.Sources |= InformationSource.OnVista;
//             return stock;
//         }
//
//         // use finanzen.net fallback to find isin
//         var isin = await _finanzenNetClient.GetISINBySymbolAndKeyword(call.Symbol, call.Name);
//         isin ??= await _finanzenNetClient.GetISINBySymbolAndKeyword(call.Symbol, call.Symbol);
//         if (string.IsNullOrWhiteSpace(isin))
//             return null;
//
//         call.Sources |= InformationSource.FinanzenNet;
//         stock = await _onVistaClient.GetStockByISIN(isin, cancellationToken);
//         if (stock == null)
//             return null;
//
//         call.Sources |= InformationSource.OnVista;
//         return stock;
//     }
// }