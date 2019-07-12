using System;

namespace TaskData.Contracts
{
     public interface ITask
     {
          string ID { get; }
          string Description { get; set; }
          bool IsFinished { get; }

          DateTime TimeCreated { get; }
          DateTime TimeLastOpened { get; }
          DateTime TimeClosed { get; }

          void CloseTask();
          void ReOpenTask();
     }
}