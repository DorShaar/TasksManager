using CommandLine;

namespace Tasker.Options
{
    public class CommandLineOptions
    {
        [Verb("get", HelpText = "Get object (tasks, groups, notes)")]
        public class GetOptions
        {
            [Value(0, HelpText = "Object type (task, group, note)")]
            public string ObjectType { get; set; }

            [Value(1, HelpText = "Object name or id")]
            public string ObjectName { get; set; }

            [Option('a', "all", HelpText = "Print all objects, even the closed ones")]
            public bool ShouldPrintAll { get; set; }

            [Option('o', "open", HelpText = "Print all open objects, not only default")]
            public bool ShouldPrintNotOnlyDefault { get; set; }

            [Option('d', "detail", HelpText = "Print more information about each object")]
            public bool IsDetailed { get; set; }

            [Option('s', "status", HelpText = "Print all tasks/groups in given status")]
            public string Status { get; set; }

            [Option('t', "days", HelpText = "Print all tasks from the last given days")]
            public int Days { get; set; }
        }

        [Verb("create", HelpText = "Create objects (tasks, groups, notes...)")]
        public class CreateOptions
        {
            [Value(0, HelpText = "Object type (task, group, note)")]
            public string ObjectType { get; set; }

            [Value(1, HelpText = "Object name or id")]
            public string ObjectName { get; set; }

            [Option('m', "message", HelpText = "Description message about the object")]
            public string Description { get; set; }
        }

        [Verb("remove", HelpText = "Removes object (tasks, groups)")]
        public class RemoveOptions
        {
            [Value(0, HelpText = "Object type to remove")]
            public string ObjectType { get; set; }

            [Value(1, HelpText = "Object id to remove")]
            public string ObjectId { get; set; }

            // TODO delete
            //[Option('h', "hard", HelpText = "Remove task group with all inner tasks")]
            //public bool ShouldHardDelete { get; set; }
        }

        [Verb("close", HelpText = "Close object (task - marks task status as closed)")]
        public class CloseOptions
        {
            [Value(0, HelpText = "Object to close (task)")]
            public string ObjectType { get; set; }

            [Value(1, HelpText = "Id of task to close")]
            public string ObjectId { get; set; }

            [Option('m', "message", HelpText = "Reason message for closing the object")]
            public string Reason { get; set; }
        }

        [Verb("move", HelpText = "Moves a task to a given group")]
        public class MoveTaskOptions
        {
            [Value(0, HelpText = "Object to move (task)")]
            public string ObjectType { get; set; }

            [Value(1, HelpText = "Task id to move")]
            public string TaskId { get; set; }

            [Value(1, HelpText = "Task group to move the task to")]
            public string TaskGroup { get; set; }
        }

        [Verb("reopen", HelpText = "Reopen a closed task")]
        public class ReOpenTaskOptions
        {
            [Value(0, HelpText = "Object to re-open (task)")]
            public string ObjectType { get; set; }

            [Value(1, HelpText = "Task id to open again")]
            public string TaskId { get; set; }

            [Option('m', "message", HelpText = "Reason message for reopening the task")]
            public string Reason { get; set; }
        }

        [Verb("work", HelpText = "Mark task as on work")]
        public class OnWorkTaskOptions
        {
            [Value(0, HelpText = "Object to set status on-work (task)")]
            public string ObjectType { get; set; }

            [Value(1, HelpText = "Task id to mark as on work")]
            public string TaskId { get; set; }

            [Option('m', "message", HelpText = "Reason message for working the task")]
            public string Reason { get; set; }
        }

        [Verb("open", HelpText = "Open note with the default text editor")]
        public class OpenNoteOptions
        {
            [Value(0, HelpText = "Object to open (note, general note)")]
            public string ObjectType { get; set; }

            [Value(0, HelpText = "Note subject or task id to open the note")]
            public string NoteName { get; set; }
        }
    }
}