using System;
using System.Collections.Generic;
using System.Text;

namespace NUTMonitor.DB.UpsStatus
{
    public class UpsMesurement
    {
        public Guid SessionID { get; set; }
        public DateTime Timestamp { get; set; }

        public int? BatteryCharge { get; set; }
        public decimal? BatteryVolt { get; set; }

        public decimal? InputVoltage { get; set; }
        public decimal? OutputVolatge { get; set; }
        public decimal? Temperature { get; set; }
        public int? StatusID { get; set; }

        public MonitorSession MonitorSession { get; set; }
        public UpsStatus StatusDescription { get; set; }
    }
}
