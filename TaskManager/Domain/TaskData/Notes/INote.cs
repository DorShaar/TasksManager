namespace TaskData.Notes
{
    public interface INote
    {
        string Subject { get; }
        string Extension { get; }
        string Text { get; }
    }
}