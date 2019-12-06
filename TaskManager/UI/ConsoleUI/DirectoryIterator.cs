using System;
using System.IO;
using System.Linq;
using TaskData.Contracts;

namespace ConsoleUI
{
    internal class DirectoryIterator
    {
        private readonly ConsolePrinter mConsolePrinter = new ConsolePrinter();
        private readonly INotesSubject mRootNotesDirectory;
        private INotesSubject mCurrentNotesDirectory;

        public DirectoryIterator(INotesSubject rootNotesDirectory)
        {
            mRootNotesDirectory = rootNotesDirectory;
            mCurrentNotesDirectory = rootNotesDirectory;
        }

        public INote Iterate(string notePath)
        {
            INote note = GetNoteFromPath(notePath);

            string userInput = string.Empty;
            while (userInput.ToLower() != "exit" && userInput.ToLower() != "q" && note == null)
            {
                PrintSubjectAndNotes();
                userInput = Console.ReadLine();

                if (userInput.ToLower().Equals(".."))
                    GoBack();
                else
                {
                    if (userInput.ToLower().StartsWith("cd "))
                        userInput = userInput.Substring("cd ".Length);

                    string fileOrDirectory = Path.Combine(mCurrentNotesDirectory.NoteSubjectFullPath, userInput);
                    if (Directory.Exists(fileOrDirectory))
                        GoIntoDirectory(userInput);
                    else
                        note = GetNote(userInput);
                }
            }

            return note;
        }

        private INote GetNoteFromPath(string path)
        {
            if (path != null)
            {
                string[] splitedNotePath = path.Split(Path.DirectorySeparatorChar);
                foreach (string subPath in splitedNotePath)
                {
                    string fileOrDirectory = Path.Combine(mCurrentNotesDirectory.NoteSubjectFullPath, subPath);
                    if (Directory.Exists(fileOrDirectory))
                        GoIntoDirectory(subPath);
                    else
                        return GetNote(subPath);
                }
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

        private INote GetNote(string noteName)
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