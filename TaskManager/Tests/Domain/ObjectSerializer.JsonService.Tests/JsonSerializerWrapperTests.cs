using FakeItEasy;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TaskData.IDsProducer;
using TaskData.ObjectSerializer.JsonService;
using TaskData.OperationResults;
using TaskData.TasksGroups;
using TaskData.TasksGroups.Producers;
using TaskData.WorkTasks.Producers;
using Triangle;
using Triangle.Time;
using Xunit;
using static TaskData.ObjectSerializer.JsonService.JsonSerializerWrapper;

namespace ObjectSerializer.JsonService.Tests
{
    public class JsonSerializerWrapperTests
    {
        private const string TestFilesDirectory = "TestFiles";
        private readonly WorkTaskProducer mWorkTaskProducer = new WorkTaskProducer();
        private readonly TasksGroupProducer mTasksGroupProducer = new TasksGroupProducer();

        [Fact]
        public async Task Serialize_AsExpected()
        {
            JsonSerializerWrapper jsonSerializerWrapper = new JsonSerializerWrapper();

            TaskGroupFactory tasksGroupFactory = new TaskGroupFactory(
                A.Fake<IIDProducer>(), NullLogger<TaskGroupFactory>.Instance);

            OperationResult<ITasksGroup> tasksGroupA = tasksGroupFactory.CreateGroup("a group", mTasksGroupProducer);
            tasksGroupFactory.CreateTask(tasksGroupA.Value, "task 1", mWorkTaskProducer);

            OperationResult<ITasksGroup> tasksGroupB = tasksGroupFactory.CreateGroup("b group", mTasksGroupProducer);
            tasksGroupFactory.CreateTask(tasksGroupB.Value, "task 3", mWorkTaskProducer);

            List<ITasksGroup> entities = new List<ITasksGroup>
            {
                tasksGroupA.Value, tasksGroupB.Value
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

            OperationResult<ITasksGroup> tasksGroupA = tasksGroupFactory.CreateGroup("a group", mTasksGroupProducer);
            string taskId = tasksGroupFactory.CreateTask(tasksGroupA.Value, "task 1", mWorkTaskProducer).Value.ID;

            TaskTriangleBuilder taskTriangleBuilder = new TaskTriangleBuilder();
            taskTriangleBuilder.SetTime("18/10/2020".ToDateTime(), TimeSpan.FromDays(3.5))
                               .AddContent("todo 1")
                               .AddContent("todo 2")
                               .AddResource("one developer");

            tasksGroupA.Value.SetMeasurement(taskId, taskTriangleBuilder.Build());
            tasksGroupA.Value.GetTask(taskId).Value.TaskMeasurement.Content.MarkContentDone("todo 2");

            List<ITasksGroup> entities = new List<ITasksGroup>
            {
                tasksGroupA.Value,
            };

            string tempSerializedFile = Path.GetRandomFileName();
            try
            {
                await jsonSerializerWrapper.Serialize(entities, tempSerializedFile).ConfigureAwait(false);

                string text = await File.ReadAllTextAsync(tempSerializedFile).ConfigureAwait(false);

                Assert.Contains("\"StartTime\": \"2020-10-18T00:00:00\"", text);
                Assert.Contains("\"ExpectedDuration\": \"3.12:00:00\"", text);
                Assert.Contains("\"todo 1\": false", text);
                Assert.Contains("\"todo 2\": true", text);
                Assert.Contains("\"one developer\"", text);
            }
            finally
            {
                File.Delete(tempSerializedFile);
            }
        }

        [Theory]
        [InlineData("tasks.db", 2)]
        [InlineData("tasks_with_triangle.db", 1)]
        public async Task Deserialize_DatabaseFromFile_AsExpected(string databaseName, int groupsCount)
        {
            string databasePath = Path.Combine(TestFilesDirectory, databaseName);

            JsonSerializerWrapper jsonSerializerWrapper = new JsonSerializerWrapper();
            jsonSerializerWrapper.RegisterConverters(new TaskConverter());
            jsonSerializerWrapper.RegisterConverters(new TaskGroupConverter());
            jsonSerializerWrapper.RegisterConverters(new TaskStatusHistoryConverter());

            List<ITasksGroup> entities =
                await jsonSerializerWrapper.Deserialize<List<ITasksGroup>>(databasePath)
                .ConfigureAwait(false);

            Assert.Equal(groupsCount, entities.Count);
        }
    }
}