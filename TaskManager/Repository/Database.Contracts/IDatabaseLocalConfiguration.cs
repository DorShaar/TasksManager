namespace Database.Contracts
{
    public interface IDatabaseLocalConfiguration
    {
        string DatabaseDirectoryPath { get; }
        string NotesDirectoryPath { get; }
        string NotesTasksDirectoryPath { get; }
    }
}