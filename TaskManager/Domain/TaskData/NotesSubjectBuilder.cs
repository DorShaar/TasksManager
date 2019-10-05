using System.IO;
using TaskData.Contracts;

namespace TaskData
{
    public class NotesSubjectBuilder : INotesSubjectBuilder
    {
        public INotesSubject Load(string notePath)
        {
            return new NotesSubject(Path.GetDirectoryName(notePath), Path.GetFileNameWithoutExtension(notePath));
        }
    }
}