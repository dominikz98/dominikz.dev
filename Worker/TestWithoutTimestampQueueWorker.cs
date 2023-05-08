// using System.Diagnostics.CodeAnalysis;
// using Microsoft.Extensions.Logging;
// using Worker.Contracts;
// using Worker.Hubs;
//
// namespace Worker;
//
// public static class TestQueueWorker
// {
//     public static void Init()
//     {
//         QueueWorkerHub.Add(new TestWithoutTimestampQueueWorker(), "Test without timestamp executed!");
//         QueueWorkerHub.Add(new TestTimestampQueueWorker(), "Test with timestamp executed!");
//     }
// }
//
// [SuppressMessage("ReSharper", "TemplateIsNotCompileTimeConstantProblem")]
// public class TestWithoutTimestampQueueWorker : QueueWorker
// {
//     public override DateTime? Timestamp { get; } = null;
//
//     public override Task Execute(ILogger logger, string? payload, CancellationToken cancellationToken)
//     {
//         logger.LogInformation(payload);
//         return Task.CompletedTask;
//     }
// }
//
// [SuppressMessage("ReSharper", "TemplateIsNotCompileTimeConstantProblem")]
// public class TestTimestampQueueWorker : QueueWorker
// {
//     public override DateTime? Timestamp { get; } = DateTime.Now.AddMinutes(1);
//
//     public override Task Execute(ILogger logger, string? payload, CancellationToken cancellationToken)
//     {
//         logger.LogInformation(payload);
//         return Task.CompletedTask;
//     }
// }