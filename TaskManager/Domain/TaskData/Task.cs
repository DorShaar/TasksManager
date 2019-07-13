using Logger.Contracts;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using TaskData.Contracts;

namespace TaskData
{
     public class Task : ITask
     {
          [JsonProperty]
          private readonly ILogger mLogger;

          [JsonProperty]
          private INote mNote;

          public string ID { get; }
          public string Description { get; set; } = string.Empty;
          public bool IsFinished { get; private set; } = false;

          public DateTime TimeCreated { get; } = DateTime.Now;
          public DateTime TimeLastOpened { get; private set; } = DateTime.Now;
          public DateTime TimeClosed { get; private set; }

          public Task(string description, ILogger logger)
          {
               mLogger = logger;
               ID = IDProducer.IDProducer.ProduceID();
               Description = description;
               mLogger?.Log($"New task id {ID} created with description: {Description}");
          }

          [JsonConstructor]
          internal Task(ILogger logger, string id, string description, bool isFinished, INote note,
                         DateTime timeCreated, DateTime timeLastOpened, DateTime timeClosed)
          {
               mLogger = logger;
               mNote = note;

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
               mLogger?.Log($"Task {ID} closed at {TimeClosed}");
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
               mLogger?.Log($"Task {ID} re-opened at {TimeLastOpened}");
          }

          public void CreateNote(string noteDirectoryPath, string content)
          {
               if (mNote != null)
               {
                    mLogger.Log($"Cannot create note since note {mNote.NotePath} is already exist");
                    return;
               }

               mNote = new Note(noteDirectoryPath, ID, content);
          }

          public void OpenNote()
          {
               ProcessStartInfo startInfo = new ProcessStartInfo(mNote.NotePath)
               {
                    UseShellExecute = true
               };

               Process process = new Process
               {
                    StartInfo = startInfo
               };

               mLogger.LogInformation($"Going to start process {process.StartInfo.FileName}");
               process.Start();
               mLogger.LogInformation($"Finished running process {process.StartInfo.FileName}");
          }

          public string GetNote()
          {
               if(mNote == null)
               {
                    mLogger.Log($"Task id {ID} has no note");
                    return string.Empty;
               }

               return mNote.NoteText;
          }
     }
}