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
            PrintSubjectAndNotes(currentNotesDirectory);

            string userInput = Console.ReadLine();
            while (userInput.ToLower() != "exit" && userInput.ToLower() != "q" && note != null)
            {
                if (userInput.ToLower().Equals(".."))
                    currentNotesDirectory = GoBack(currentNotesDirectory);
                else
                {
                    string fileOrDirectory = Path.Combine(currentNotesDirectory.NoteSubjectFullPath, userInput);
                    if (Directory.Exists(fileOrDirectory))
                        currentNotesDirectory = InsertDirectory(currentNotesDirectory, userInput);
                    else if (File.Exists(fileOrDirectory))
                        note = GetNote(currentNotesDirectory, userInput);
                }

                if(note != null)
                {
                    PrintSubjectAndNotes(currentNotesDirectory);
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
                if (notesSubject.NoteSubjectName == directory)
                    return notesSubject;
            }

            return null;
        }

        private INote GetNote(INotesSubject notesDirectory, string noteName)
        {
            foreach (INote note in notesDirectory.GetNotes())
            {
                if (Path.GetFileName(note.NotePath).Equals(noteName))
                    return note;
                if (Path.GetFileNameWithoutExtension(note.NotePath).Equals(noteName))
                    return note;
            }

            return null;
        }
    }
}