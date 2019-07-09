using Logger.Contracts;
using System;

namespace Logger
{
     public class ConsoleLogger : ILogger
     {
          private const string Separetor = "############################################";

          public void Log(string message)
          {
               Console.WriteLine(message);
          }

          public void LogError(string message)
          {
               Log(Separetor);
               Console.ForegroundColor = ConsoleColor.Red;
               Log(message);
               Console.ResetColor();
               Log(Separetor);
          }

          public void LogError(string message, Exception ex)
          {
               Log(Separetor);
               Console.ForegroundColor = ConsoleColor.Red;
               Log(message);
               Log($"Exception Message: {ex.Message}");
               Log($"Exception Message: {ex.Message}");
               Console.ResetColor();
               Log(Separetor);
          }
     }
}