using FakeItEasy;
using Logger.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;

namespace TaskData.Tests
{
     [TestClass]
     public class TaskTests
     {
          private readonly ILogger mLogger = A.Dummy<ILogger>();
          private readonly string DummyGroupName = "dummy group";
          private readonly string DummyDescription = "dummy description";
          private readonly int SleepTimeInMs = 2000;
          private readonly string NotesDirectory = "TempNotesDirecory";

          [TestMethod]
          public void CreateNewTask_TimeCreatedIsNow()
          {
               Task task = new Task(DummyGroupName, DummyDescription, mLogger);
               Assert.IsTrue(IsTimesAlmostTheSame(task.TaskStatusHistory.TimeCreated, DateTime.Now));
          }

          [TestMethod]
          public void CloseTask_IsFinished_True()
          {
               Task task = new Task(DummyGroupName, DummyDescription, mLogger);
               Assert.IsFalse(task.IsFinished);
               task.CloseTask(string.Empty);
               Assert.IsTrue(task.IsFinished);
          }

          [TestMethod]
          public void CloseTask_TimeCloseIsNow()
          {
               Task task = new Task(DummyGroupName, DummyDescription, mLogger);
               task.CloseTask(string.Empty);
               Assert.IsTrue(IsTimesAlmostTheSame(task.TaskStatusHistory.TimeClosed, DateTime.Now));
          }

          [TestMethod]
          public void CloseTask_ClosedTask_NoChange()
          {
               Task task = new Task(DummyGroupName, DummyDescription, mLogger);
               task.CloseTask(string.Empty);
               DateTime closeTime = task.TaskStatusHistory.TimeClosed;
               task.CloseTask(string.Empty);
               Assert.AreEqual(task.TaskStatusHistory.TimeClosed, closeTime);
          }

          [TestMethod]
          public void ReOpenTask_TimeLastOpenIsNow()
          {
               Task task = new Task(DummyGroupName, DummyDescription, mLogger);
               task.CloseTask(string.Empty);
               Thread.Sleep(SleepTimeInMs);
               task.ReOpenTask(string.Empty);
               Assert.IsTrue(IsTimesAlmostTheSame(task.TaskStatusHistory.TimeLastOpened, DateTime.Now));
          }

          [TestMethod]
          public void ReOpenTask_TimCreated_NoChange()
          {
               Task task = new Task(DummyGroupName, DummyDescription, mLogger);
               DateTime createdTime = task.TaskStatusHistory.TimeCreated;
               task.CloseTask(string.Empty);
               task.ReOpenTask(string.Empty);
               Assert.AreEqual(task.TaskStatusHistory.TimeCreated, createdTime);
          }

          [TestMethod]
          public void ReOpenTask_IsFinished_False()
          {
               Task task = new Task(DummyGroupName, DummyDescription, mLogger);
               task.CloseTask(string.Empty);
               task.ReOpenTask(string.Empty);
               Assert.IsFalse(task.IsFinished);
          }

          [TestMethod]
          public void ReOpeneTask_OpenedTask_NoChange()
          {
               Task task = new Task(DummyGroupName, DummyDescription, mLogger);
               DateTime createdTime = task.TaskStatusHistory.TimeLastOpened;
               task.ReOpenTask(string.Empty);
               Assert.AreEqual(task.TaskStatusHistory.TimeLastOpened, createdTime);
          }

          [TestMethod]
          public void CreateNote_AlreadyCreated_NoCreation()
          {
               string excpectedText = "should not be deleted";
               string notePath = null;
               try
               {
                    Task task = new Task(DummyGroupName, DummyDescription, mLogger);
                    notePath = task.CreateNote(NotesDirectory, excpectedText);

                    task.CreateNote(NotesDirectory, "another content");

                    Assert.AreEqual(excpectedText, File.ReadAllText(notePath));
               }
               finally
               {
                    File.Delete(notePath);
                    Directory.Delete(NotesDirectory, recursive: true);
               }
          }

          private bool IsTimesAlmostTheSame(DateTime time1, DateTime time2)
          {
               return (time1 - time2).Seconds < 2;
          }
     }
}
