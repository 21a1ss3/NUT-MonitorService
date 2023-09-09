using System;
using System.Collections.Generic;
using System.Text;

namespace NUTMonitor.UPS
{
    public class TaskFailLogEntry : LogEntry
    {
        private string _getTaskNameMsgPart() => string.IsNullOrWhiteSpace(TaskName) ? string.Empty : $" with name '{TaskName}'";

        public override string Message => $"Task '{TaskType}'{_getTaskNameMsgPart()} has failed with exception:\r\n\r\n{Exception.ToString()}";

        public string TaskType { get; internal set; }
        public string TaskName { get; internal set; }
        public Exception Exception { get; internal set; }
    }
}
