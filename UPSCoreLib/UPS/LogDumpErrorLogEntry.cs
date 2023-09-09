using System;
using System.Collections.Generic;
using System.Text;

namespace NUTMonitor.UPS
{
    public class LogDumpErrorLogEntry : LogEntry
    {
        public string FileName { get; set; }
        public Exception Exception { get; internal set; }
        public override string Message => $"Failed to dump log (file name {FileName} on disk with exception:\r\n\r\n{Exception}";
    }
}
