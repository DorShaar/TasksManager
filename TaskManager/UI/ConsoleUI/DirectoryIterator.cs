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

        public DirectoryIterator(INotesSubject rootNotesDirectory)
        {
            mRootNotesDirectory = rootNotesDirectory;
        }

        public INote Iterate(string notePath)
        {
            INote note = null;
            INotesSubject currentNotesDirectory = mRootNotesDirectory;

            if (notePath != null)
            {
                string[] splitedNotePath = notePath.Split(Path.DirectorySeparatorChar);
                foreach (string subPath in splitedNotePath)
                {
                    string fileOrDirectory = Path.Combine(currentNotesDirectory.NoteSubjectFullPath, subPath);
                    if (Directory.Exists(fileOrDirectory))
                        currentNotesDirectory = InsertDirectory(currentNotesDirectory, subPath);
                    else
                        note = GetNote(currentNotesDirectory, subPath);
                }
            }

            string userInput = string.Empty;
            while (userInput.ToLower() != "exit" && userInput.ToLower() != "q" && note == null)
            {
                PrintSubjectAndNotes(currentNotesDirectory);
                userInput = Console.ReadLine();

                if (userInput.ToLower().Equals(".."))
                    currentNotesDirectory = GoBack(currentNotesDirectory);
                else
                {
                    if (userInput.ToLower().StartsWith("cd "))
                        userInput = userInput.Substring("cd ".Length);

                    string fileOrDirectory = Path.Combine(currentNotesDirectory.NoteSubjectFullPath, userInput);
                    if (Directory.Exists(fileOrDirectory))
                        currentNotesDirectory = InsertDirectory(currentNotesDirectory, userInput);
                    else
                        note = GetNote(currentNotesDirectory, userInput);
                }
            }

            return note;
        }

        private void PrintSubjectAndNotes(INotesSubject notesDirectory)
        {
            mConsolePrinter.Print(notesDirectory.GetNotesSubjects().Select(
                subject => Path.GetFileName(subject.NoteSubjectFullPath)), "SUBJECTS");
            mConsolePrinter.Print(notesDirectory.GetNotes().Select(
                note => Path.GetFileName(note.NotePath)), "NOTES");
            mConsolePrinter.Print(string.Empty, string.Empty);
        }

        private INotesSubject GoBack(INotesSubject notesDirectory)
        {
            //string topDirectoryPath = mRootNotesDirectory.NoteSubjectFullPath;
            //string directoryPath = Path.GetDirectoryName(notesDirectory.NoteSubjectFullPath);

            //if (topDirectoryPath.Equals(directoryPath))
            //    return mRootNotesDirectory;
            //else
            return mRootNotesDirectory;
        }

        private INotesSubject InsertDirectory(INotesSubject notesDirectory, string directory)
        {
            foreach (INotesSubject notesSubject in notesDirectory.GetNotesSubjects())
            {
                if (notesSubject.NoteSubjectName.Equals(directory, StringComparison.OrdinalIgnoreCase))
                    return notesSubject;
            }

            if (directory.Equals(".."))
                return GoBack(notesDirectory);

            return notesDirectory;
        }

        private INote GetNote(INotesSubject notesDirectory, string noteName)
        {
            foreach (INote note in notesDirectory.GetNotes())
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