namespace TaskData.Contracts
{
    public interface INoteBuilder
    {
        INote CreateNote(string directoryPath, string noteName, string content);
        INote Load(string notePath);
    }
}