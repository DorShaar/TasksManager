using System;
using TaskData.Contracts;

namespace TaskData
{
     public class Task : ITask
     {
          public string ID { get; } = IDCounter.GetNextID();
          public string Description { get; set; } = string.Empty;
          public bool IsFinished { get; private set; } = false;

          public DateTime TimeCreated { get; } = DateTime.Now;
          public DateTime TimeLastOpened { get; private set; } = DateTime.Now;
          public DateTime TimeClosed { get; private set; }

          public Task(string description)
          {
               Description = description;
          }

          public void CloseTask()
          {
               IsFinished = true;
               TimeClosed = DateTime.Now;
          }

          public void ReOpen()
          {
               IsFinished = false;
               TimeLastOpened = DateTime.Now;
          }
     }
}