using System;
using System.Threading.Tasks;
using TaskData.WorkTasks;
using Xunit;

namespace TaskData.Tests.WorkTasks
{
    public class WorkTaskTests
    {
        private const string DummyDescription = "dummy description";
        private const int SleepTimeInMs = 2000;

        [Fact]
        public void CreateNewTask_TimeCreatedIsNow()
        {
            WorkTask task = new WorkTask("1000", DummyDescription);
            Assert.True(IsTimesAlmostTheSame(task.TaskStatusHistory.TimeCreated, DateTime.Now));
        }

        [Fact]
        public void CloseTask_IsFinished_True()
        {
            WorkTask task = new WorkTask("1000", DummyDescription);
            Assert.False(task.IsFinished);
            task.CloseTask(string.Empty);
            Assert.True(task.IsFinished);
        }

        [Fact]
        public void CloseTask_TimeCloseIsNow()
        {
            WorkTask task = new WorkTask("1000", DummyDescription);
            task.CloseTask(string.Empty);
            Assert.True(IsTimesAlmostTheSame(task.TaskStatusHistory.TimeClosed, DateTime.Now));
        }

        [Fact]
        public void CloseTask_ClosedTask_NoChange()
        {
            WorkTask task = new WorkTask("1000", DummyDescription);
            task.CloseTask(string.Empty);
            DateTime closeTime = task.TaskStatusHistory.TimeClosed;
            task.CloseTask(string.Empty);
            Assert.Equal(task.TaskStatusHistory.TimeClosed, closeTime);
        }

        [Fact]
        public async Task ReOpenTask_TimeLastOpenIsNow()
        {
            WorkTask task = new WorkTask("1000", DummyDescription);
            task.CloseTask(string.Empty);

            await Task.Delay(SleepTimeInMs).ConfigureAwait(false);
            task.ReOpenTask(string.Empty);

            Assert.True(IsTimesAlmostTheSame(task.TaskStatusHistory.TimeLastOpened, DateTime.Now));
        }

        [Fact]
        public void ReOpenTask_TimCreated_NoChange()
        {
            WorkTask task = new WorkTask("1000", DummyDescription);
            DateTime createdTime = task.TaskStatusHistory.TimeCreated;
            task.CloseTask(string.Empty);
            task.ReOpenTask(string.Empty);
            Assert.Equal(task.TaskStatusHistory.TimeCreated, createdTime);
        }

        [Fact]
        public void ReOpenTask_IsFinished_False()
        {
            WorkTask task = new WorkTask("1000", DummyDescription);
            task.CloseTask(string.Empty);
            task.ReOpenTask(string.Empty);
            Assert.False(task.IsFinished);
        }

        [Fact]
        public void ReOpeneTask_OpenedTask_NoChange()
        {
            WorkTask task = new WorkTask("1000", DummyDescription);
            DateTime createdTime = task.TaskStatusHistory.TimeLastOpened;
            task.ReOpenTask(string.Empty);
            Assert.Equal(task.TaskStatusHistory.TimeLastOpened, createdTime);
        }

        private bool IsTimesAlmostTheSame(DateTime time1, DateTime time2)
        {
            return (time1 - time2).Seconds < 2;
        }
    }
}