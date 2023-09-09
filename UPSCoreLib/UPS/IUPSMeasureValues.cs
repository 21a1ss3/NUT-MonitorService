using System;
using System.Collections.Generic;
using System.Text;

namespace NUTMonitor.UPS
{
    public interface IUPSMeasureValues
    {
        public int? BatteryCharge { get; }
        public decimal? BatteryVolt { get; }

        public decimal? InputVoltage { get; }
        public decimal? OutputVolatge { get; }
        public decimal? Temperature { get; }
        public UPSStatus Status { get; }
    }
}
