using System.IO;
using TaskData.Contracts;

namespace TaskData
{
    public class NotesSubjectBuilder : INotesSubjectBuilder
    {
        public INotesSubject Load(string notePath)
        {
            if (!File.Exists(notePath))
                return null;
            else
                return new NotesSubject(Path.GetDirectoryName(notePath), Path.GetFileNameWithoutExtension(notePath));
        }
    }
}