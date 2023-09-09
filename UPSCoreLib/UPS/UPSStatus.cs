using System;
using System.Collections.Generic;
using System.Text;

namespace NUTMonitor.UPS
{
    public class UPSStatus
    {
        public string StringName { get; set; }

        public bool IsConnectedToAC { get; internal set; }

        public bool IsWellKnown { get; internal set; }
    }
}
