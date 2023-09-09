using System;
using System.Collections.Generic;
using System.Text;

namespace NUTMonitor.UPS
{
    public class UPSMeasure : IUPSMeasureValues
    {
        public int? BatteryCharge { get; set; }
        public decimal? BatteryVolt { get; set; }
        public decimal? InputVoltage { get; set; }
        public decimal? OutputVolatge { get; set; }
        public decimal? Temperature { get; set; }
        public decimal BatteryMinVolt { get; set; }
        public decimal BatteryHighVolt { get; set; }

        public UPSStatus Status { get; set; }
        public UPSMeasureStatus ProcessingStatus { get; private set; } = new UPSMeasureStatus();
        public DateTime Timestamp { get; private set; }

        public bool Validate()
        {
            bool res = true;

            if (BatteryCharge == null)
                res = false;

            if (BatteryVolt == null)
                res = false;

            if (InputVoltage == null)
                res = false;

            if (OutputVolatge == null)
                res = false;

            if (Temperature == null)
                res = false;

            if (Status == null)
                res = false;

            return res;
        }

        public UPSMeasureDifference GetDifferenceWith(UPSMeasure other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (!Validate())
                throw new Exception("This instance should be valid");

            if (!other.Validate())
                throw new Exception("The other instance should be valid");

            UPSMeasureDifference diff = new UPSMeasureDifference();

            if (this == other)
                return diff;

            int diffCount = 0;

            if (BatteryCharge.Value != other.BatteryCharge.Value)
            {
                diffCount++;
                diff.BatteryCharge = other.BatteryCharge.Value;

            }

            if (BatteryVolt.Value != other.BatteryVolt.Value)
                if (Math.Abs(BatteryVolt.Value - other.BatteryVolt.Value) > 0.1M)
                {
                    diffCount++;
                    diff.BatteryVolt = other.BatteryVolt.Value;
                }

            if (InputVoltage.Value != other.InputVoltage.Value)
                if ((Math.Abs(InputVoltage.Value - other.InputVoltage.Value) > 6) || (InputVoltage.Value <= 220) || (InputVoltage.Value > 250))
                {

                    diffCount++;
                    diff.InputVoltage = other.InputVoltage.Value;
                }


            if (OutputVolatge.Value != other.OutputVolatge.Value)
                if ((Math.Abs(OutputVolatge.Value - other.OutputVolatge.Value) > 6))
                {
                    diffCount++;
                    diff.OutputVolatge = other.OutputVolatge.Value;
                }

            if (Temperature.Value != other.Temperature.Value)
            {
                diffCount++;
                diff.Temperature = other.Temperature.Value;
            }

            if (Status.StringName != other.Status.StringName)
            {
                diffCount++;
                diff.Status = other.Status;
            }

            return diff;
        }
    }
}
