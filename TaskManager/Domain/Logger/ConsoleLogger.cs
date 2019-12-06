using Logger.Contracts;
using System;

namespace Logger
{
     public class ConsoleLogger : ILogger
     {
          public bool ShouldLogInformation { get; set; } = false;

          public void Log(string message)
          {
               Console.WriteLine(message);
          }

          public void LogInformation(string message)
          {
               if(ShouldLogInformation)
                    Console.WriteLine(message);
          }

          public void LogError(string message)
          {
               Console.ForegroundColor = ConsoleColor.Red;
               Log(message);
               Console.ResetColor();
          }

          public void LogError(string message, Exception ex)
          {
               Console.ForegroundColor = ConsoleColor.Red;
               Log(message);
               Log($"Exception Message: {ex.Message}");
               Log($"Exception Message: {ex.StackTrace}");
               Console.ResetColor();
          }
     }
}