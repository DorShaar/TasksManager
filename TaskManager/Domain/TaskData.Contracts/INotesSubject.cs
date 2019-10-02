using System.Collections.Generic;

namespace TaskData.Contracts
{
    public interface INotesSubject
    {
        IEnumerable<INotesSubject> GetNotesSubjects();
        IEnumerable<INote> GetNotes();
        string NoteSubject { get; }

        void AddNoteSubject(string noteSubject);
        void AddNote(string directoryPath, string noteName, string content);
    }
}