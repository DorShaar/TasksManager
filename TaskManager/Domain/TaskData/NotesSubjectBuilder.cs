using System;
using System.IO;
using TaskData.Contracts;

namespace TaskData
{
    public class NotesSubjectBuilder : INotesSubjectBuilder
    {
        public INotesSubject Load(string noteSubjectPath)
        {
            if (!Directory.Exists(noteSubjectPath))
                throw new Exception($"No note subject {noteSubjectPath} exist");

            return new NotesSubject(Path.GetDirectoryName(noteSubjectPath), Path.GetFileNameWithoutExtension(noteSubjectPath));
        }
    }
}