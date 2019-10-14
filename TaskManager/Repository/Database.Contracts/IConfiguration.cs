namespace Database.Contracts
{
    public interface IConfiguration
    {
        string DatabaseDirectoryPath { get; }
        void SetDatabaseDirectoryPath(string newDatabaseDirectoryPath);

        string NotesDirectoryPath { get; }
        void SetNotesDirectoryPath(string newNotesDirectoryPath);

        string NotesTasksDirectoryPath { get; }
    }
}