using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace TaskData.Tests
{
     [TestClass]
     public class TaskTests
     {
          private readonly string DummyDescription = "dummy description";
          private readonly int SleepTimeInMs = 2000;

          [TestMethod]
          public void CreateNewTask_TimeCreatedIsNow()
          {
               Task task = new Task(DummyDescription);
               Assert.IsTrue(IsTimesAlmostTheSame(task.TimeCreated, DateTime.Now));
          }

          [TestMethod]
          public void CloseTask_IsFinished_True()
          {
               Task task = new Task(DummyDescription);
               Assert.IsFalse(task.IsFinished);
               task.CloseTask();
               Assert.IsTrue(task.IsFinished);
          }

          [TestMethod]
          public void CloseTask_TimeCloseIsNow()
          {
               Task task = new Task(DummyDescription);
               task.CloseTask();
               Assert.IsTrue(IsTimesAlmostTheSame(task.TimeClosed, DateTime.Now));
          }

          [TestMethod]
          public void CloseTask_ClosedTask_NoChange()
          {
               Task task = new Task(DummyDescription);
               task.CloseTask();
               DateTime closeTime = task.TimeClosed;
               task.CloseTask();
               Assert.AreEqual(task.TimeClosed, closeTime);
          }

          [TestMethod]
          public void ReOpenTask_TimeLastOpenIsNow()
          {
               Task task = new Task(DummyDescription);
               task.CloseTask();
               Thread.Sleep(SleepTimeInMs);
               task.ReOpenTask();
               Assert.IsTrue(IsTimesAlmostTheSame(task.TimeLastOpened, DateTime.Now));
          }

          [TestMethod]
          public void ReOpenTask_TimCreated_NoChange()
          {
               Task task = new Task(DummyDescription);
               DateTime createdTime = task.TimeCreated;
               task.CloseTask();
               task.ReOpenTask();
               Assert.AreEqual(task.TimeCreated, createdTime);
          }

          [TestMethod]
          public void ReOpenTask_IsFinished_False()
          {
               Task task = new Task(DummyDescription);
               task.CloseTask();
               task.ReOpenTask();
               Assert.IsFalse(task.IsFinished);
          }

          [TestMethod]
          public void ReOpeneTask_OpenedTask_NoChange()
          {
               Task task = new Task(DummyDescription);
               DateTime createdTime = task.TimeLastOpened;
               task.ReOpenTask();
               Assert.AreEqual(task.TimeLastOpened, createdTime);
          }

          private bool IsTimesAlmostTheSame(DateTime time1, DateTime time2)
          {
               return (time1 - time2).Seconds < 2;
          }
     }
}
