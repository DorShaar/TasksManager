﻿using System.Collections.Generic;
using System.IO;

namespace TaskData.Notes
{
    internal class NotesSubject : INotesSubject
    {
        private readonly INoteFactory mNoteBuilder = new NoteFactory();
        private readonly List<string> mNotesExtensions = new List<string> { ".txt" };
        private readonly string mNoteSubjectDirectory;
        public string NoteSubjectName { get; }
        public string NoteSubjectFullPath => Path.Combine(mNoteSubjectDirectory, NoteSubjectName);

        internal NotesSubject(string directoryPath, string noteSubjectName)
        {
            mNoteSubjectDirectory = directoryPath;
            NoteSubjectName = noteSubjectName;
            Directory.CreateDirectory(NoteSubjectFullPath);
        }

        public IEnumerable<INote> GetNotes()
        {
            foreach (string notePath in Directory.EnumerateFiles(NoteSubjectFullPath))
            {
                if (mNotesExtensions.Contains(Path.GetExtension(notePath)))
                    yield return mNoteBuilder.LoadNote(notePath);
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