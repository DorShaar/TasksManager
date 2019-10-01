﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TaskData.Contracts
{
    public interface ITaskStatusHistory
    {
        List<StatusData> StatusHistory { get; }
        DateTime TimeCreated { get; }
        DateTime TimeLastOpened { get; }
        DateTime TimeLastOnWork { get; }
        DateTime TimeClosed { get; }
        Status CurrentStatus { get; }
        void AddHistory(DateTime dateTime, Status status, string reason);
    }

    public class StatusData
    {
        public DateTime DateTime { get; }
        public Status Status { get; }
        public string Reason { get; set; }

        [JsonConstructor]
        public StatusData(DateTime dateTime, Status status, string reason)
        {
            DateTime = dateTime;
            Status = status;
            Reason = reason;
        }
    }
}