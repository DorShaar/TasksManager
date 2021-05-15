using TaskData.OperationResults;

namespace TaskData.Notes
{
    public interface INotable
    {
        OperationResult CreateNote(string subject, string content);
        OperationResult<INote> GetNote();
    }
}