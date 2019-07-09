using System;

namespace Logger.Contracts
{
     public interface ILogger
     {
          void Log(string message);
          void LogError(string message);
          void LogError(string message, Exception ex);
     }
}