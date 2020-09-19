using Newtonsoft.Json;
using System;

namespace TaskData.TaskStatus
{
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