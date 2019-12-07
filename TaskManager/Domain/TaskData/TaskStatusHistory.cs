using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TaskData.Contracts;

namespace TaskData
{
    public class TaskStatusHistory : ITaskStatusHistory
    {
        [JsonProperty]
        private readonly List<StatusData> StatusHistory = new List<StatusData>();

        [JsonIgnore]
        public DateTime TimeCreated => StatusHistory[0].DateTime;

        [JsonIgnore]
        public DateTime TimeLastOpened => FindLastStatus(Status.Open);

        [JsonIgnore]
        public DateTime TimeLastOnWork => FindLastStatus(Status.OnWork);

        [JsonIgnore]
        public DateTime TimeClosed => FindLastStatus(Status.Closed);

        [JsonIgnore]
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