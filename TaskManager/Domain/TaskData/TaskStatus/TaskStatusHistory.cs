using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ObjectSerializer.JsonService")]
[assembly: InternalsVisibleTo("Composition")]
namespace TaskData.TaskStatus
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class TaskStatusHistory : ITaskStatusHistory
    {
        [JsonProperty]
        private readonly List<StatusData> StatusHistory = new List<StatusData>();

        public DateTime TimeCreated => StatusHistory[0].DateTime;

        public DateTime TimeLastOpened => FindLastStatus(Status.Open);

        public DateTime TimeLastOnWork => FindLastStatus(Status.OnWork);

        public DateTime TimeClosed => FindLastStatus(Status.Closed);

        public Status CurrentStatus => StatusHistory.Last().Status;

        private DateTime FindLastStatus(Status status)
        {
            StatusData lastStatusData = StatusHistory.FindLast(statusData => statusData.Status == status);
            if (lastStatusData != null)
                return lastStatusData.DateTime;

            return default;
        }

        public void AddHistory(DateTime dateTime, Status status, string reason)
        {
            StatusHistory.Add(new StatusData(dateTime, status, reason));
        }
    }
}