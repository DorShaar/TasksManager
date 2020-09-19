using System.IO;

namespace TaskData.Notes
{
    internal class NoteFactory : INoteFactory
    {
        public INote CreateNote(string directoryPath, string noteName, string content)
        {
            return new Note(directoryPath, noteName, content);
        }

        public INote LoadNote(string notePath)
        {
            return new Note(Path.GetDirectoryName(notePath), Path.GetFileNameWithoutExtension(notePath));
        }

        public INotesSubject LoadNotesSubject(string noteSubjectPath)
        {
            if (!Directory.Exists(noteSubjectPath))
                throw new DirectoryNotFoundException($"No note subject {noteSubjectPath} exist");

            return new NotesSubject(Path.GetDirectoryName(noteSubjectPath), Path.GetFileNameWithoutExtension(noteSubjectPath));
        }
    }
}