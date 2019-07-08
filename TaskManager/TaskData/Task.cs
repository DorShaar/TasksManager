using Logger.Contracts;
using Newtonsoft.Json;
using System;
using TaskData.Contracts;

namespace TaskData
{
     public class Task : ITask
     {
          [JsonProperty]
          private readonly ILogger mLogger;

          public string ID { get; }
          public string Description { get; set; } = string.Empty;
          public bool IsFinished { get; private set; } = false;

          public DateTime TimeCreated { get; } = DateTime.Now;
          public DateTime TimeLastOpened { get; private set; } = DateTime.Now;
          public DateTime TimeClosed { get; private set; }

          public Task(string description, ILogger logger)
          {
               mLogger = logger;
               ID = IDCounter.GetNextID();
               Description = description;
               mLogger?.Log($"New task id {ID} created with description: {Description}");
          }

          [JsonConstructor]
          internal Task(ILogger logger, string id, string description, bool isFinished,
                         DateTime timeCreated, DateTime timeLastOpened, DateTime timeClosed)
          {
               mLogger = logger;
               ID = id;
               Description = description;
               IsFinished = isFinished;
               TimeCreated = timeCreated;
               TimeLastOpened = timeLastOpened;
               TimeClosed = timeClosed;

               mLogger?.Log($"Task id {ID} restored");
          }

          public void CloseTask()
          {
               if (IsFinished)
               {
                    mLogger?.Log($"Task {ID} is already closed");
                    return;
               }

               IsFinished = true;
               TimeClosed = DateTime.Now;
          }

          public void ReOpenTask()
          {
               if (!IsFinished)
               {
                    mLogger?.Log($"Task {ID} is already open");
                    return;
               }

               IsFinished = false;
               TimeLastOpened = DateTime.Now;
          }
     }
}