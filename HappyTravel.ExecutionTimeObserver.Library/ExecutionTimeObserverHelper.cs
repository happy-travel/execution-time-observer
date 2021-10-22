using System;
using System.Threading;
using System.Threading.Tasks;

namespace HappyTravel.ExecutionTimeObserver.Library
{
    public static class ExecutionTimeObserverHelper
    {
        public static async Task<T> Execute<T>(Func<Task<T>> funcToExecute, Func<Task> funcToNotify, TimeSpan notifyAfter)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            
            Task.Run(async () =>
            {
                await Task.Delay(notifyAfter, cancellationTokenSource.Token);
                await funcToNotify();
            });
            
            var result = await funcToExecute();
            cancellationTokenSource.Cancel();
            return result;
        }
    }
}