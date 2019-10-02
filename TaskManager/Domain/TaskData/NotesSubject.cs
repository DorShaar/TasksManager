using System.Collections.Generic;
using System.IO;
using TaskData.Contracts;

namespace TaskData
{
    public class NotesSubject : INotesSubject
    {
        private readonly List<INotesSubject> mNotesSubjects = new List<INotesSubject>();
        private readonly List<INote> mNotes = new List<INote>();
        private readonly string mNoteSubjectPath;
        public string NoteSubject { get; }

        public NotesSubject(string directoryPath, string noteSubject)
        {
            NoteSubject = noteSubject;
            mNoteSubjectPath = Directory.CreateDirectory(Path.Combine(directoryPath, noteSubject)).FullName;
        }

        public IEnumerable<INote> GetNotes()
        {
            return mNotes;
        }

        public IEnumerable<INotesSubject> GetNotesSubjects()
        {
            return mNotesSubjects;
        }

        public void AddNote(string directoryPath, string noteName, string content)
        {
            mNotes.Add(new Note(directoryPath, noteName, content));
        }

        public void AddNoteSubject(string noteSubject)
        {
            mNotesSubjects.Add(new NotesSubject(mNoteSubjectPath, noteSubject));
        }
    }
}