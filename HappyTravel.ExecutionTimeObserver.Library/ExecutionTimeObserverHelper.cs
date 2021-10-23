using System;
using System.Threading;
using System.Threading.Tasks;
using HappyTravel.ExecutionTimeObserver.Library.Exceptions;

namespace HappyTravel.ExecutionTimeObserver.Library
{
    public static class ExecutionTimeObserverHelper
    {
        public static async Task<T> Execute<T>(Func<Task<T>> funcToExecute, Func<Task> funcToNotify, TimeSpan notifyAfter)
        {
            if (funcToExecute == null || funcToNotify == null)
                throw new ArgumentNullException();

            if (notifyAfter < TimeSpan.Zero)
                throw new NegativeDelayException("Negative delay not supported");
            
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