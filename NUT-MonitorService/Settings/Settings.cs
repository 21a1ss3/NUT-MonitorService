using System;
using System.Collections.Generic;
using System.Text;

namespace NUTMonitor.Settings
{
    public class Settings
    {
        public LogsSettings Logging { get; set; } = new LogsSettings();

        public UpsSettings UPS { get; set; } = new UpsSettings();

        public EsxiSettings Esxi { get; set; } = new EsxiSettings();
    }
}
