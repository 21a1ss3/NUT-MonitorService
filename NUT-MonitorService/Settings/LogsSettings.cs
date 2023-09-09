using System;
using System.Collections.Generic;
using System.Text;

namespace NUTMonitor.Settings
{
    public class LogsSettings
    {
        public string UnhandledLogPath { get; set; } = @"/var/log/upsmon/dumped";

        public int InMemoryLimit { get; set; } = 100;

        public int MaxAttemptsToProcessEntry { get; set; } = 50;

        public TimeSpan FailProcessDelay { get; set; }
    }
}
