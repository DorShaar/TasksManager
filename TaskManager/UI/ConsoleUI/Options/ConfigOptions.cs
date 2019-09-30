using CommandLine;

namespace ConsoleUI.Options
{
    internal class ConfigOptions
    {
        [Verb("set-db", HelpText = "Set database file path")]
        public class SetDatabasePathOptions
        {
            [Value(0)]
            public string NewDatabasePath { get; set; }
        }

        [Verb("get-db", HelpText = "Get databases path")]
        public class GetDatabasePathOptions
        {
        }
    }
}