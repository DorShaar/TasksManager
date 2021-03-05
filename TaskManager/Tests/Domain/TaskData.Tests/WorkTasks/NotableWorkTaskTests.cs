using TaskData.WorkTasks;
using Xunit;

namespace TaskData.Tests.WorkTasks
{
    public class NotableWorkTaskTests
    {
        private const string DummyDescription = "dummy description";
        private const string NotesDirectory = "TempNotesDirecory";

        [Fact]
        public void CreateNote_AlreadyCreated_NoCreation()
        {
            NotableWorkTask task = new NotableWorkTask("1000", DummyDescription);

            Assert.True(task.CreateNote(NotesDirectory, "first text").Success);

            Assert.False(task.CreateNote(NotesDirectory, "another content").Success);
        }
    }
}