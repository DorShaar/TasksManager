namespace Database.Contracts
{
    public interface ILocalRepository<T> : IRepository<T>
    {
        string DatabasePath { get; }
        string NotesDatabaseDirectoryPath { get; }
        string NotesTasksDatabaseDirectoryPath { get; }
    }
}