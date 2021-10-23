using System;
using System.Threading;
using System.Threading.Tasks;
using HappyTravel.ExecutionTimeObserver.Exceptions;

namespace HappyTravel.ExecutionTimeObserver
{
    public static class TimeObserver
    {
        public static async Task<T> Execute<T>(Func<Task<T>> observedFunc, Func<Task> notifyFunc, TimeSpan notifyAfter)
        {
            if (observedFunc == null || notifyFunc == null)
                throw new ArgumentNullException();

            if (notifyAfter < TimeSpan.Zero)
                throw new NegativeDelayException("Negative delay not supported");
            
            var cancellationTokenSource = new CancellationTokenSource();
            
            Task.Run(async () =>
            {
                await Task.Delay(notifyAfter, cancellationTokenSource.Token);
                await notifyFunc();
            });
            
            var result = await observedFunc();
            cancellationTokenSource.Cancel();
            return result;
        }
    }
}