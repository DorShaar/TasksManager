using System;

namespace Logger
{
     public class ConsoleLogger : ILogger
     {
          public void Log(string message)
          {
               Console.WriteLine(message);
          }

          public void LogError(string message)
          {
               Console.ForegroundColor = ConsoleColor.Red;
               Log(message);
               Console.ResetColor();
          }
     }
}