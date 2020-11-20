namespace TaskData.Notes
{
    public interface INote
    {
        string Extension { get; }
        string NotePath { get; }
        string Text { get; }
    }
}