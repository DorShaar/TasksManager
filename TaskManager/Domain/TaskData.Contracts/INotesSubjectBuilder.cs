namespace TaskData.Contracts
{
    public interface INotesSubjectBuilder
    {
        INotesSubject Load(string notePath);
    }
}