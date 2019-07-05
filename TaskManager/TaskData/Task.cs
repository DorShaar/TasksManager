using System;
using TaskData.Contracts;

namespace TaskData
{
     public class Task : ITask
     {
          public string ID { get; } = IDCounter.GetNextID();
          public ITaskGroup TaskFamily { get; set; } = null;
          public string Description { get; set; } = string.Empty;
          public bool IsFinished { get; private set; } = false;

          public DateTime TimeCreated { get; } = DateTime.Now;
          public DateTime TimeLastOpened { get; private set; } = DateTime.Now;
          public DateTime TimeClosed { get; private set; }

          public Task(ITaskGroup taskFamily, string description)
          {
               TaskFamily = taskFamily;
               Description = description;
          }

          public Task(string description) : this(null, description)
          {
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