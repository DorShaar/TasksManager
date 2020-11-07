namespace TaskData.Notes
{
    public delegate void OpenNoteHandler(INote note);

    public interface INote
    {
        string Extension { get; }
        string NotePath { get; }
        string Text { get; }

        event OpenNoteHandler OpenRequested;
    }
}