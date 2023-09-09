using System;
using System.Collections.Generic;
using System.Text;

namespace NUTMonitor.Settings
{
    public class UpsSettings
    {
        public TimeSpan ProbingInterval { get; set; } = new TimeSpan(0, 0, 2);
    }
}
