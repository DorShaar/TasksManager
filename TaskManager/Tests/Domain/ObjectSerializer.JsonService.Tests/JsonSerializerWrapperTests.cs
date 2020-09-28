using FakeItEasy;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TaskData.IDsProducer;
using TaskData.TasksGroups;
using Triangle;
using Triangle.Time;
using Xunit;

namespace ObjectSerializer.JsonService.Tests
{
    public class JsonSerializerWrapperTests
    {
        private const string TestFilesDirectory = "TestFiles";

        [Fact]
        public async Task Serialize_AsExpected()
        {
            JsonSerializerWrapper jsonSerializerWrapper = new JsonSerializerWrapper();

            TaskGroupFactory tasksGroupFactory = new TaskGroupFactory(
                A.Fake<IIDProducer>(), NullLogger<TaskGroupFactory>.Instance);

            ITasksGroup tasksGroupA = tasksGroupFactory.CreateGroup("a group");
            tasksGroupFactory.CreateTask(tasksGroupA, "task 1");

            ITasksGroup tasksGroupB = tasksGroupFactory.CreateGroup("b group");
            tasksGroupFactory.CreateTask(tasksGroupB, "task 3");

            List<ITasksGroup> entities = new List<ITasksGroup>
            {
                tasksGroupA, tasksGroupB
            };

            string tempSerializedFile = Path.GetRandomFileName();
            try
            {
                await jsonSerializerWrapper.Serialize(entities, tempSerializedFile).ConfigureAwait(false);

                string text = await File.ReadAllTextAsync(tempSerializedFile).ConfigureAwait(false);

                Assert.Contains("\"GroupName\": \"a group\"", text);
                Assert.Contains("\"Description\": \"task 1\"", text);
                Assert.Contains("\"GroupName\": \"b group\"", text);
                Assert.Contains("\"Description\": \"task 3\"", text);
            }
            finally
            {
                File.Delete(tempSerializedFile);
            }
        }

        [Fact]
        public async Task Serialize_WithTaskMeasurement_AsExpected()
        {
            JsonSerializerWrapper jsonSerializerWrapper = new JsonSerializerWrapper();

            TaskGroupFactory tasksGroupFactory = new TaskGroupFactory(
                A.Fake<IIDProducer>(), NullLogger<TaskGroupFactory>.Instance);

            ITasksGroup tasksGroupA = tasksGroupFactory.CreateGroup("a group");
            string taskId = tasksGroupFactory.CreateTask(tasksGroupA, "task 1").ID;

            TaskTriangleBuilder taskTriangleBuilder = new TaskTriangleBuilder();
            taskTriangleBuilder.SetTime("18/10/2020", DayPeriod.Morning, 3, halfWorkDay: true)
                               .AddContent("todo 1")
                               .AddContent("todo 2")
                               .AddPercentageProgressToNotify(60)
                               .AddResource("one developer");

            tasksGroupA.SetMeasurement(taskId, taskTriangleBuilder.Build());

            List<ITasksGroup> entities = new List<ITasksGroup>
            {
                tasksGroupA,
            };

            string tempSerializedFile = Path.GetRandomFileName();
            try
            {
                await jsonSerializerWrapper.Serialize(entities, tempSerializedFile).ConfigureAwait(false);

                string text = await File.ReadAllTextAsync(tempSerializedFile).ConfigureAwait(false);

          //      "TaskMeasurement": {
          //          "Configuration": {
          //              "PercentagesProgressToNotify": [
          //                60
          //            ]
          //},
          //"Time": {
          //              "TimeMode": 0,
          //  "StartTime": {
          //                  "DateTime": "2020-10-18T00:00:00",
          //    "DayPeriod": 0
          //  },
          //  "ExpectedDuration": {
          //                  "Days": 3,
          //    "Hours": 12
          //  }
          //          },
          //"Content": { },
          //"Resources": { }

                    Assert.Contains("\"GroupName\": \"a group\"", text);
                Assert.Contains("\"Description\": \"task 1\"", text);
                Assert.Contains("\"GroupName\": \"b group\"", text);
                Assert.Contains("\"Description\": \"task 3\"", text);
            }
            finally
            {
                File.Delete(tempSerializedFile);
            }
        }

        [Theory]
        [InlineData(@"tasks.db", 2)]
        //[InlineData(@"tasks_with_triangle.db")]
        public async Task Deserialize_DatabaseFromFile_AsExpected(string databaseName, int groupsCount)
        {
            string databasePath = Path.Combine(TestFilesDirectory, databaseName);

            JsonSerializerWrapper jsonSerializerWrapper = new JsonSerializerWrapper();

            List<ITasksGroup> entities = 
                await jsonSerializerWrapper.Deserialize<List<ITasksGroup>>(databasePath)
                .ConfigureAwait(false);

            Assert.Equal(groupsCount, entities.Count);
        }
    }
}