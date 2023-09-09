using System;
using System.Collections.Generic;
using System.Text;

namespace NUTMonitor.UPS
{
    public abstract class LogEntry
    {
        internal LogEntry()
        {
            Timestampt = DateTime.UtcNow;
        }

        public Guid EntryId { get; private set; } = Guid.NewGuid();
        public DateTime Timestampt { get; private set; }
        public abstract string Message { get; }

        public int ProcessedTimes { get; private set; }
        public DateTime LastProcessedTime { get; private set; }

        internal void MarkAsProcessed()
        {
            ProcessedTimes++;
            LastProcessedTime = DateTime.UtcNow;
        }
    }
}
