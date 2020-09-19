using System.Collections.Generic;

namespace TaskData.Notes
{
    public interface INotesSubject
    {
        string NoteSubjectName { get; }
        string NoteSubjectFullPath { get; }

        IEnumerable<INote> GetNotes();
        IEnumerable<INotesSubject> GetNotesSubjects();
        void AddNoteSubject(string noteSubject);
        void AddNote(string directoryPath, string noteName, string content);
    }
}