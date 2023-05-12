using dominikz.Worker.Hubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// bind to ctrl + c
var cancellationSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, _) => cancellationSource.Cancel();

// create configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var loggerFactory = LoggerFactory.Create(builder => builder
    .SetMinimumLevel(LogLevel.Information)
    .AddFile(options =>
    {
        options.Directory = "logs";
        options.UseRollingFiles = true;
        options.RollingFileTimestampFormat = "yyyy-MM-dd";
        options.FileExtension = "log";
        options.FileNamePrefix = "worker";
        options.MinimumLogLevel = LogLevel.Information;
    })
    .AddConsole());

var host = new WorkerHost(loggerFactory, configuration, cancellationSource.Token);
await host.Start();