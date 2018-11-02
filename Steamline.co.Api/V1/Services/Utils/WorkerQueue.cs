using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Steamline.co.Api.V1.Services.Utils
{
    public interface IWorkerQueue
    {
        void EnqueueAsync(Func<CancellationToken, Task> workItem);
        Task<T> EnqueueAsync<T>(Func<TaskCompletionSource<T>, CancellationToken, Task> workItem);

        Task<Func<CancellationToken, Task>> DequeueAsync(
            CancellationToken cancellationToken);
    }

    public class WorkerQueue : IWorkerQueue
    {
        private ConcurrentQueue<Func<CancellationToken, Task>> _workItems = 
            new ConcurrentQueue<Func<CancellationToken, Task>>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void EnqueueAsync(
            Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems.Enqueue(workItem);
            _signal.Release();
        }

        public Task<T> EnqueueAsync<T>(
            Func<TaskCompletionSource<T>, CancellationToken, Task> workItemAsync)
        {
            if (workItemAsync == null)
            {
                throw new ArgumentNullException(nameof(workItemAsync));
            }

            var promise = new TaskCompletionSource<T>();
            Func<CancellationToken, Task> wrapper = async token => {
                await workItemAsync(promise, token);
            };

            _workItems.Enqueue(wrapper);
            _signal.Release();

            return promise.Task;
        }

        public async Task<Func<CancellationToken, Task>> DequeueAsync(
            CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }
    }
}