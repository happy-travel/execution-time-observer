using System;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace HappyTravel.ExecutionTimeObserver.UnitTests
{
    public class ExecutionTimeObserverTests
    {
        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(10)]
        private async Task Result_should_be_equal(int result)
        {
            var notifyFunc = new Mock<Func<Task>>();
            var value = await TimeObserver.Execute(observedFunc: () => LongRunningFunction(3, result),
                notifyFunc: notifyFunc.Object,
                notifyAfter: TimeSpan.FromSeconds(1));
            
            Assert.Equal(result, value);
        }


        [Theory]
        [InlineData(5, 2, 10)]
        [InlineData(2, 1, 100)]
        [InlineData(3, 2, 1000)]
        private async Task Notify_should_not_be_run(int runningTime, int notifyAfter, int result)
        {
            if (notifyAfter > runningTime)
                throw new Exception("Notify time should be greater than running time");
            
            var notifyFunc = new Mock<Func<Task>>();
            var value = await TimeObserver.Execute(observedFunc: () => LongRunningFunction(3, result),
                notifyFunc: notifyFunc.Object,
                notifyAfter: TimeSpan.FromSeconds(5));
            
            notifyFunc.Verify(x => x(), Times.Never);
            Assert.Equal(result, value);
        }


        [Theory]
        [InlineData(2, 5, 10)]
        [InlineData(1, 3, 100)]
        [InlineData(3, 5, 1000)]
        private async Task Notify_should_be_run(int runningTime, int notifyAfter, int result)
        {
            if (runningTime > notifyAfter)
                throw new Exception("Running time should be greater than notify time");
            
            var notifyFunc = new Mock<Func<Task>>();
            var value = await TimeObserver.Execute(observedFunc: () => LongRunningFunction(5, result),
                notifyFunc: notifyFunc.Object,
                notifyAfter: TimeSpan.FromSeconds(2));
            
            notifyFunc.Verify(x => x(), Times.Once);
            Assert.Equal(result, value);
        }


        [Fact]
        private async Task Null_observed_function_should_throws_exception()
        {
            var notifyFunc = new Mock<Func<Task>>();
            var task = TimeObserver.Execute<Task<int>>(observedFunc: () => null,
                notifyFunc: notifyFunc.Object,
                notifyAfter: TimeSpan.FromSeconds(2));

            await Assert.ThrowsAsync<NullReferenceException>(() => task);
        }
        
        
        [Fact]
        private async Task Null_notify_function_should_throws_exception()
        {
            var task = TimeObserver.Execute(observedFunc: () => LongRunningFunction(3, 0),
                notifyFunc: null,
                notifyAfter: TimeSpan.FromSeconds(2));

            await Assert.ThrowsAsync<ArgumentNullException>(() => task);
        }


        [Fact]
        private async Task Negative_delay_should_throws_exception()
        {
            var notifyFunc = new Mock<Func<Task>>();
            var task = TimeObserver.Execute(observedFunc: () => LongRunningFunction(5, 0),
                notifyFunc: notifyFunc.Object,
                notifyAfter: TimeSpan.FromSeconds(-2));
            
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => task);
        }


        private static async Task<int> LongRunningFunction(int seconds = 0, int result= 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            return result;
        }
    }
}