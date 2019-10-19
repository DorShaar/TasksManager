using Database.Contracts;

namespace Database.Configuration
{
    public class DatabaseLocalConfigurtaion : IDatabaseLocalConfiguration
    {
        public string DatabaseDirectoryPath { get; set; }
        public string NotesDirectoryPath { get; set; }
        public string NotesTasksDirectoryPath { get; set; }
    }
}