using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaskData.Notes;
using UI.ConsolePrinter;

namespace ConsoleUI
{
    internal class NotesDirectoryIterator
    {
        private readonly ConsolePrinter mConsolePrinter;
        private readonly INotesSubject mRootNotesDirectory;
        private INotesSubject mCurrentNotesDirectory;

        public NotesDirectoryIterator(INotesSubject rootNotesDirectory, ConsolePrinter consolePrinter)
        {
            mRootNotesDirectory = rootNotesDirectory;
            mCurrentNotesDirectory = rootNotesDirectory;
            mConsolePrinter = consolePrinter;
        }

        public INote GetNote(string notePath)
        {
            INote note = GetNoteFromPath(notePath);

            if (note != null)
                return note;

            string userInput = string.Empty;
            while (!string.Equals(userInput, "exit", StringComparison.OrdinalIgnoreCase) &&
                   !string.Equals(userInput, "q", StringComparison.OrdinalIgnoreCase) &&
                   note == null)
            {
                PrintSubjectAndNotes();
                userInput = Console.ReadLine();
                note = IterateByUserInput(userInput);
            }

            return note;
        }

        private INote IterateByUserInput(string userInput)
        {
            if (userInput.Equals("..", StringComparison.OrdinalIgnoreCase))
            {
                GoBack();
            }
            else
            {
                if (userInput.StartsWith("cd ", StringComparison.OrdinalIgnoreCase))
                    userInput = userInput.Substring("cd ".Length);

                return GetNoteOrChangeDirectory(userInput);
            }

            return null;
        }

        private INote GetNoteFromPath(string path)
        {
            if (path != null)
            {
                string[] splitedNotePath = path.Split(Path.DirectorySeparatorChar);
                foreach (string subPath in splitedNotePath)
                {
                    return GetNoteOrChangeDirectory(subPath);
                }
            }

            return null;
        }

        private INote GetNoteOrChangeDirectory(string subPath)
        {
            string fileOrDirectory = Path.Combine(mCurrentNotesDirectory.NoteSubjectFullPath, subPath);
            if (Directory.Exists(fileOrDirectory))
            {
                GoIntoDirectory(subPath);
            }
            else
            {
                IEnumerable<string> possibleNotes = Directory.EnumerateFiles(mCurrentNotesDirectory.NoteSubjectFullPath)
                    .Where(fileName => fileName.Contains(subPath));

                if (possibleNotes.Count() == 1)
                    return GetNoteByName(Path.GetFileName(possibleNotes.First()));
                else
                    mConsolePrinter.Print(possibleNotes.Select(noteFullPath => Path.GetFileName(noteFullPath)), "POSSIBLE NOTES");
            }

            return null;
        }

        private void PrintSubjectAndNotes()
        {
            mConsolePrinter.Print(mCurrentNotesDirectory.GetNotesSubjects().Select(
                subject => Path.GetFileName(subject.NoteSubjectFullPath)), "SUBJECTS");
            mConsolePrinter.Print(mCurrentNotesDirectory.GetNotes().Select(
                note => Path.GetFileName(note.NotePath)), "NOTES");
            mConsolePrinter.Print(string.Empty, string.Empty);
        }

        private void GoBack()
        {
            mCurrentNotesDirectory = mRootNotesDirectory;
        }

        private void GoIntoDirectory(string directory)
        {
            foreach (INotesSubject notesSubject in mCurrentNotesDirectory.GetNotesSubjects())
            {
                if (notesSubject.NoteSubjectName.Equals(directory, StringComparison.OrdinalIgnoreCase))
                    mCurrentNotesDirectory = notesSubject;
            }
        }

        private INote GetNoteByName(string noteName)
        {
            foreach (INote note in mCurrentNotesDirectory.GetNotes())
            {
                if (Path.GetFileName(note.NotePath).Equals(noteName, StringComparison.OrdinalIgnoreCase))
                    return note;
                if (Path.GetFileNameWithoutExtension(note.NotePath).Equals(noteName, StringComparison.OrdinalIgnoreCase))
                    return note;
            }

            return null;
        }
    }
}