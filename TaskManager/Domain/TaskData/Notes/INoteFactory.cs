namespace TaskData.Notes
{
    public interface INoteFactory
    {
        INote CreateNote(string directoryPath, string noteName, string content);
        INote LoadNote(string notePath);
        INotesSubject LoadNotesSubject(string noteSubjectPath);
    }
}