using CommandLine;

namespace ConsoleUI.Options
{
    internal class NotesOptions
    {
        [Verb("create note", HelpText = "Create note to a given task")]
        public class CreateNoteOptions
        {
            [Value(0, HelpText = "Task or group id to create the note")]
            public string TaskId { get; set; }

            [Value(1, HelpText = "Text to write in the note")]
            public string Text { get; set; }
        }

        [Verb("create general note", HelpText = "Create general note")]
        public class CreateGeneralNoteOptions
        {
            [Value(0, HelpText = "The name / subject of the note")]
            public string NoteSubject { get; set; }

            [Value(1, HelpText = "Text to write in the note")]
            public string Text { get; set; }
        }

        [Verb("get notes", HelpText = "Get all notes names")]
        public class GetNotesOptions
        {
            [Option('a', "all", HelpText = "Print all notes, even non-general")]
            public bool ShouldPrintAllNotes { get; set; }
        }

        [Verb("open note", HelpText = "Open note with the default text editor")]
        public class OpenNoteOptions
        {
            [Value(0, HelpText = "Note subject or task id to open the note")]
            public string NoteName { get; set; }
        }

        [Verb("get note", HelpText = "Get note string text")]
        public class GetNoteOptions
        {
            [Value(0, HelpText = "Note subject or task id to get the note text")]
            public string NoteName { get; set; }
        }
    }
}