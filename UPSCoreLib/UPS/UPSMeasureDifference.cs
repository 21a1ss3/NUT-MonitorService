using System;
using System.Collections.Generic;
using System.Text;

namespace NUTMonitor.UPS
{
    public class UPSMeasureDifference : IUPSMeasureValues
    {
        public int? BatteryCharge { get; set; }
        public decimal? BatteryVolt { get; set; }
        public decimal? InputVoltage { get; set; }
        public decimal? OutputVolatge { get; set; }
        public decimal? Temperature { get; set; }
        public UPSStatus Status { get; set; }

        public bool IsEmpty
        {
            get
            {
                return (BatteryCharge == null) &&
                    (BatteryVolt == null) &&
                    (InputVoltage == null) &&
                    (OutputVolatge == null) &&
                    (Temperature == null) &&
                    (Status == null);
            }
        }
    }
}
