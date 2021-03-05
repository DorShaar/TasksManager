using TaskData.OperationResults;

namespace TaskData.Notes
{
    public interface INotable
    {
        OperationResult CreateNote(string noteDirectoryPath, string content);
        OperationResult<string> GetNote();
    }
}