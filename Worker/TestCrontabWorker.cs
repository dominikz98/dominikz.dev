// using dominikz.Domain.Structs;
// using Microsoft.Extensions.Logging;
// using Worker.Contracts;
//
// namespace Worker;
//
// public class FirstTestCrontabWorker : CrontabWorker
// {
//     public override CronSchedule[] Schedules { get; } = new[]
//     {
//         new CronSchedule("* * * * *")
//     };
//
//     public override async Task Execute(ILogger logger, CancellationToken cancellationToken)
//     {
//         await Task.Delay(2000, cancellationToken);
//         logger.LogInformation("Finished 1");
//     }
// }
//
// public class SecondTestCrontabWorker : CrontabWorker
// {
//     public override CronSchedule[] Schedules { get; } = new[]
//     {
//         new CronSchedule("*/1 * * * *")
//     };
//
//     public override async Task Execute(ILogger logger, CancellationToken cancellationToken)
//     {
//         await Task.Delay(500, cancellationToken);
//         logger.LogInformation("Finished 2");
//     }
// }