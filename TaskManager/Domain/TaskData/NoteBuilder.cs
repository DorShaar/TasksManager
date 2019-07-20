using System.IO;
using TaskData.Contracts;

namespace TaskData
{
    public class NoteBuilder : INoteBuilder
    {
        public INote CreateNote(string directoryPath, string noteName, string content)
        {
            return new Note(directoryPath, noteName, content);
        }

        public INote Load(string notePath)
        {
            return new Note(Path.GetDirectoryName(notePath), Path.GetFileNameWithoutExtension(notePath));
        }
    }
}