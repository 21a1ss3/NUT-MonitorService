using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NUTMonitor.DB.UpsStatus
{
    public class MonitorSession
    {
        [Key]
        public Guid SessionId { get; set; }


        public DateTime Launched { get; set; }
        public DateTime? LastUpdate { get; set; }
        public TimeSpan ConfiguredInterval { get; set; }

        public decimal? BatteryMinVolt { get; set; }
        public decimal? BatteryHighVolt { get; set; }
        public int UPSId { get; set; }

        public ICollection<UpsMesurement> UpsMesurements { get; set; }
    }
}
