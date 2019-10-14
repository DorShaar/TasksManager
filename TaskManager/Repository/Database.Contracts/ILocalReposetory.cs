namespace Database.Contracts
{
    public interface ILocalReposetory<T> : IRepository<T>
    {
        string DatabasePath { get; }
        string NotesDatabaseDirectoryPath { get; }
    }
}