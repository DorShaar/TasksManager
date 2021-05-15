using ObjectSerializer.JsonService;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Integration.Tests
{
    public class JsonSerializerWrapperTests
    {
        private const string TestFilesDirectory = "TestFiles";

        [Theory]
        [InlineData("tasks.db", 2)]
        [InlineData("tasks_with_triangle.db", 1)]
        public async Task Deserialize_DatabaseFromFile_AsExpected(string databaseName, int groupsCount)
        {
            string databasePath = Path.Combine(TestFilesDirectory, databaseName);

            JsonSerializerWrapper jsonSerializerWrapper = new JsonSerializerWrapper();

            List <TasksGroup> entities =
                await jsonSerializerWrapper.Deserialize<List<TasksGroup>>(databasePath)
                .ConfigureAwait(false);

            Assert.Equal(groupsCount, entities.Count);
        }
    }
}