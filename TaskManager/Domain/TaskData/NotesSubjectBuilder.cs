using System.IO;
using TaskData.Contracts;

namespace TaskData
{
    public class NotesSubjectBuilder : INotesSubjectBuilder
    {
        public INotesSubject Load(string noteSubjectPath)
        {
            if (!Directory.Exists(noteSubjectPath))
                return null;
            else
                return new NotesSubject(Path.GetDirectoryName(noteSubjectPath), Path.GetFileNameWithoutExtension(noteSubjectPath));
        }
    }
}