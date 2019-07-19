using CommandLine;

namespace ConsoleUI.Options
{
    internal class NotesOptions
    {
        [Verb("create-note", HelpText = "Create note to a given task or tasks group")]
        public class CreateNoteOptions
        {
            [Value(0, HelpText = "Task or group id to create the note")]
            public string TaskId { get; set; }

            [Value(1, HelpText = "Text to write in the note")]
            public string Text { get; set; }
        }

        [Verb("open-note", HelpText = "Open note with the default text editor")]
        public class OpenNoteOptions
        {
            [Value(0, HelpText = "Task or group id to open the note")]
            public string TaskId { get; set; }
        }

        [Verb("get-note", HelpText = "Get note string text")]
        public class GetNoteOptions
        {
            [Value(0, HelpText = "Task or group id to get the note text")]
            public string TaskId { get; set; }
        }
    }
}