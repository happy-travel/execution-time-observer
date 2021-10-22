using System;
using System.Threading.Tasks;
using HappyTravel.ExecutionTimeObserver.Library;
using Moq;
using Xunit;

namespace HappyTravel.ExecutionTimeObserver._UnitTests
{
    public class ExecutionTimeObserverTests
    {
        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(10)]
        private async Task Result_should_be_equal(int result)
        {
            var longRunningFunction = LongRunningFunction(3, result);
            var notifyFunc = new Mock<Func<Task>>();
            var value = await ExecutionTimeObserverHelper.Execute(funcToExecute: () => longRunningFunction,
                funcToNotify: notifyFunc.Object,
                notifyAfter: TimeSpan.FromSeconds(1));
            
            Assert.Equal(result, value);
        }


        [Fact]
        private async Task Notify_should_not_be_run()
        {
            var longRunningFunction = LongRunningFunction(3);
            var notifyFunc = new Mock<Func<Task>>();
            var value = await ExecutionTimeObserverHelper.Execute(funcToExecute: () => longRunningFunction,
                funcToNotify: notifyFunc.Object,
                notifyAfter: TimeSpan.FromSeconds(5));
            
            notifyFunc.Verify(x => x(), Times.Never);
        }


        [Fact]
        private async Task Notify_should_be_run()
        {
            var longRunningFunction = LongRunningFunction(5);
            var notifyFunc = new Mock<Func<Task>>();
            var value = await ExecutionTimeObserverHelper.Execute(funcToExecute: () => longRunningFunction,
                funcToNotify: notifyFunc.Object,
                notifyAfter: TimeSpan.FromSeconds(2));
            
            notifyFunc.Verify(x => x(), Times.Once);
        }


        private static async Task<int> LongRunningFunction(int seconds, int result = 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            return result;
        }
    }
}