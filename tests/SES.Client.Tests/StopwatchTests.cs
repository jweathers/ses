#pragma warning disable CA2007 //DO NOT call configureawait in test harness
using Moq;
using SES.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SES.Client.Tests
{
    public class StopwatchTests
    {
        [Fact]
        public async Task StopwatchCreatedStateYieldsElapsedTimeCorrectlyTestAsync()
        {
            var swStarted = new Stopwatch(true);
            var swNotStarted = new Stopwatch(false);
            await Task.Delay(50);
            swStarted.Stop();
            swNotStarted.Stop();
            Assert.NotEqual(TimeSpan.Zero,swStarted.ElapsedTime);
            Assert.Equal(TimeSpan.Zero,swNotStarted.ElapsedTime);
        }

        [Fact]
        public async Task StopwatchCanBeStartedTestAsync()
        {
            var swNotStarted = new Stopwatch(false);
            swNotStarted.Start();
            await Task.Delay(50);
            swNotStarted.Stop();
            Assert.NotEqual(TimeSpan.Zero,swNotStarted.ElapsedTime);            
        }

        [Fact]
        public async Task StopwatchCanBeStoppedTestAsync()
        {
            var swNotStarted = new Stopwatch(false);
            swNotStarted.Start();
            await Task.Delay(50);
            swNotStarted.Stop();
            var expectedElapsedTime = swNotStarted.ElapsedTime;
            await Task.Delay(50);
            Assert.Equal(expectedElapsedTime,swNotStarted.ElapsedTime);            
        }

        [Fact]
        public async Task SplitTest()
        {
            var sw1 = new Stopwatch();
            const string splitName = nameof(splitName);
            sw1.Split(splitName);
            await Task.Delay(50);
            Assert.NotEqual(TimeSpan.Zero,sw1.SplitTime(splitName));


        }
        [Fact]
        public async Task RestartIfRunningTest()
        {
            var sw = new Stopwatch(true);
            await Task.Delay(50);
            sw.Start(true);
            sw.Stop();
            Assert.True(sw.ElapsedTime<TimeSpan.FromMilliseconds(50));

            var sw2 = new Stopwatch(true);
            await Task.Delay(50);
            Assert.Throws<InvalidOperationException>(()=>sw2.Start(false));
            sw.Stop();
            Assert.NotEqual(TimeSpan.Zero,sw.ElapsedTime);
        }

        [Fact]
        public async Task ThrowExceptionWhenElapsedTimeIsAccessedBeforeStoppage()
        {
           var sw = new Stopwatch(true);
            await Task.Delay(50);
            sw.Start(true);
            Assert.Throws<InvalidOperationException>(()=>sw.ElapsedTime);            
        }
    }
}
#pragma warning restore CA2007