using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Steamline.co.Api.V1.Helpers;
using Steamline.co.Api.V1.Services.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Steamline.co.Api.V1.Services
{
    public class WorkerQueueHostedService : IHostedService
    {
        private CancellationTokenSource _shutdown = new CancellationTokenSource();
        private Task _backgroundTask;
        private readonly ILogger _logger;

        private IWorkerQueue _taskQueue;

        public WorkerQueueHostedService(IWorkerQueue taskQueue, ILogger<WorkerQueueHostedService> logger)
        {
            _taskQueue = taskQueue;
            _logger = logger;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Log(LogLevel.Information, new EventId((int)LogEventId.General), "Queued Hosted Service is starting.");

            _backgroundTask = Task.Run(BackgroundProceessing);

            return Task.CompletedTask;
        }

        private async Task BackgroundProceessing()
        {
            while (!_shutdown.IsCancellationRequested)
            {
                var workItem = await _taskQueue.DequeueAsync(_shutdown.Token);

                try
                {
                    await workItem(_shutdown.Token);
                }
                catch (Exception)
                {
                    _logger.Log(LogLevel.Error, new EventId((int)LogEventId.General), $"Error occurred executing {nameof(workItem)}.");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Log(LogLevel.Information, new EventId((int)LogEventId.General), "Queued Hosted Service is stopping.");

            _shutdown.Cancel();

            return Task.WhenAny(_backgroundTask,
                Task.Delay(Timeout.Infinite, cancellationToken));
        }
    }
}