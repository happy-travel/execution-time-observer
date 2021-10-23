using System;
using System.Threading;
using System.Threading.Tasks;

namespace HappyTravel.ExecutionTimeObserver
{
    public static class TimeObserver
    {
        public static async Task<T> Execute<T>(Func<Task<T>> observedFunc, Func<Task> notifyFunc, TimeSpan notifyAfter)
        {
            if (observedFunc is null)
                throw new ArgumentNullException(nameof(observedFunc));

            if (notifyFunc is null)
                throw new ArgumentNullException(nameof(notifyFunc));

            if (notifyAfter < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(notifyAfter), "Delay has negative value");
            
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            
            _ = Task.Run(async () =>
            {
                await Task.Delay(notifyAfter, cancellationToken);
                await notifyFunc();
            }, cancellationToken);
            
            var result = await observedFunc();
            cancellationTokenSource.Cancel();
            return result;
        }
    }
}