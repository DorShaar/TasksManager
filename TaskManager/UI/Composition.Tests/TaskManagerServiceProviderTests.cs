using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Contracts;

namespace Composition.Tests
{
    [TestClass]
    public class TaskManagerServiceProviderTests
    {
        [TestMethod]
        public void CreateTaskManagerService_LocalDatabasePath_AsExpected()
        {
            TaskManagerServiceProvider serviceProvider = new TaskManagerServiceProvider();
            ITaskManager taskManager = serviceProvider.GetTaskManagerService();

            Assert.AreEqual(@"C:\Users\Dor Shaar\TaskManagerApp\db\tasks.db", taskManager.GetDatabasePath());
            Assert.AreEqual(
                @"C:\Dor\My Computing Programs\DevWorld", taskManager.NotesRootDatabase.NoteSubjectFullPath);
            Assert.AreEqual(
                @"C:\Dor\My Computing Programs\DevWorld\Tasks", taskManager.NotesTasksDatabase.NoteSubjectFullPath);
        }
    }
}