// using dominikz.Api.Attributes;
// using dominikz.Api.Background;
// using dominikz.Api.Utils;
// using dominikz.Domain.Models;
// using dominikz.Infrastructure.Clients.Finance;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using OxyPlot;
// using OxyPlot.Axes;
// using ControllerBase = Microsoft.AspNetCore.Mvc.ControllerBase;
//
// namespace dominikz.Api.Endpoints.Admin;
//
// [Tags("admin")]
// [ApiKey(RequiresMasterKey = true)]
// [ApiController]
// [Route("api/admin/playground")]
// public class Playground : ControllerBase
// {
//     private readonly IServiceProvider _serviceProvider;
//
//     public Playground(IServiceProvider serviceProvider)
//     {
//         _serviceProvider = serviceProvider;
//     }
//
//     [HttpPost] 
//     public async Task<IActionResult> Execute(CancellationToken cancellationToken)
//     {
//         var data = new Dictionary<DateTime, OHLCSeriesItem>();
//         var open = new List<double>();
//         var high = new List<double>();
//         var low = new List<double>();
//         var close = new List<double>();
//         var dates = new List<DateTime>();
//
//         // Sample data
//         var json = "{\"c\":[217.68,221.03,219.89],\"h\":[222.49,221.5,220.94],\"l\":[217.19,217.1402,218.83],\"o\":[221.03,218.55,220],\"s\":\"ok\",\"t\":[1569297600,1569384000,1569470400],\"v\":[33463820,24018876,20730608]}";
//         dynamic dataJson = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
//
//         for (int i = 0; i < dataJson.t.Count; i++)
//         {
//             var date = DateTimeOffset.FromUnixTimeSeconds(dataJson.t[i]).DateTime;
//             dates.Add(date);
//             open.Add((double)dataJson.o[i]);
//             high.Add((double)dataJson.h[i]);
//             low.Add((double)dataJson.l[i]);
//             close.Add((double)dataJson.c[i]);
//             data[date] = new OHLCSeriesItem((double)dataJson.o[i], (double)dataJson.h[i], (double)dataJson.l[i], (double)dataJson.c[i]);
//         }
//
//         var plotModel = new PlotModel();
//         plotModel.Title = "Candle Chart";
//         plotModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "dd/MM/yyyy" });
//         plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left });
//
//         var series = new OHLCSeries();
//         series.Items.AddRange(data.Values);
//         plotModel.Series.Add(series);
//
//         var exporter = new PdfExporter { Width = 600, Height = 400 };
//         exporter.ExportToFile(plotModel, "candlechart.pdf");
//     }
// }
