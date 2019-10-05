using System.Collections.Generic;
using System.IO;
using TaskData.Contracts;

namespace TaskData
{
    public class NotesSubject : INotesSubject
    {
        private readonly INoteBuilder mNoteBuilder = new NoteBuilder();
        private readonly List<string> mNotesExtensions = new List<string> { ".txt" };
        public string NoteSubjectDirectory { get; private set; }
        public string NoteSubjectName { get; private set; }
        public string NoteSubjectFullPath => Path.Combine(NoteSubjectDirectory, NoteSubjectName);

        public NotesSubject(string directoryPath, string noteSubjectName)
        {
            NoteSubjectDirectory = directoryPath;
            NoteSubjectName = noteSubjectName;
            Directory.CreateDirectory(NoteSubjectFullPath);
        }

        public IEnumerable<INote> GetNotes()
        {
            foreach (string notePath in Directory.EnumerateFiles(NoteSubjectFullPath))
            {
                if (mNotesExtensions.Contains(Path.GetExtension(notePath)))
                    yield return mNoteBuilder.Load(notePath);
            }
        }

        public IEnumerable<INotesSubject> GetNotesSubjects()
        {
            foreach (string noteSubjectPath in Directory.EnumerateDirectories(NoteSubjectFullPath))
            {
                yield return new NotesSubject(NoteSubjectFullPath, Path.GetFileName(noteSubjectPath));
            }
        }

        public void AddNote(string directoryPath, string noteName, string content)
        {
            mNoteBuilder.CreateNote(directoryPath, noteName, content);
        }

        public void AddNoteSubject(string noteSubjectName)
        {
            new NotesSubject(NoteSubjectFullPath, noteSubjectName);
        }
    }
}