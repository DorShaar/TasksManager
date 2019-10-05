using TaskData.Contracts;

namespace ConsoleUI
{
    internal class DirectoryIterator
    {
        private ConsolePrinter mConsolePrinter = new ConsolePrinter();

        public INote Iterate(string notePath, INotesSubject rootNotesDirectory)
        {
            mConsolePrinter.Print(, "Subjects");
            mConsolePrinter.Print(rootNotesDirectory.GetNotesSubjects, "Subjects");

            string userInput = string.Empty;
            while(userInput.ToLower() != "exit" && userInput.ToLower() != "q")
            {

            }

            if(notePath)
        }

        public INotesSubject GoInside(INotesSubject notesDirectory, string directory)
        {
            foreach(INotesSubject notesSubject in notesDirectory.GetNotesSubjects())
            {
                if (notesSubject.NoteSubjectName == directory)
                    return notesSubject;
            }

            return null;
        }
    }
}