﻿using Logger.Contracts;
using System;
using TaskData.Contracts;

namespace TaskData
{
     public class Task : ITask
     {
          private static ILogger mLogger;

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

          public static void SetLogger(ILogger logger)
          {
               mLogger = logger;
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