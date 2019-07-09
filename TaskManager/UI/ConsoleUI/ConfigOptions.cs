using CommandLine;

namespace ConsoleUI
{
     internal class ConfigOptions
     {
          // Tasks Group.
          [Verb("set-db", HelpText = "Set database file path")]
          public class SetDatabasePathOptions
          {
               [Value(0)]
               public string NewDatabasePath { get; set; }
          }
     }
}