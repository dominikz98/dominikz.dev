using dominikz.Infrastructure.Clients;
using dominikz.Infrastructure.Worker;
using dominikz.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// bind to ctrl + c
var cancellationSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, _) => cancellationSource.Cancel();

// create configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

// auto register worker
var timeTriggeredWorker = GetTimeTriggeredWorker();

// create dependency container
await new ServiceCollection()
    .AddWorkerLogging()
    .AddWorkerOptions(configuration)
    .AddWorker(timeTriggeredWorker)
    .AddExternalClients()
    .AddProvider(configuration)
    .AddSingleton<EmailClient>()
    .AddSingleton((sp) =>
    {
        var logger = sp.GetRequiredService<ILogger<TimeTriggerPollingService>>();
        var email = sp.GetRequiredService<EmailClient>();
        return new TimeTriggerPollingService(logger, sp, email, timeTriggeredWorker);
    })
    .BuildServiceProvider(true)
    .GetRequiredService<TimeTriggerPollingService>()
    .Run(cancellationSource.Token);


static IReadOnlyCollection<Type> GetTimeTriggeredWorker()
    => typeof(TimeTriggeredWorker)
        .Assembly
        .GetTypes()
        .Where(t => typeof(TimeTriggeredWorker).IsAssignableFrom(t))
        .Where(x => !x.IsAbstract)
        .ToArray();