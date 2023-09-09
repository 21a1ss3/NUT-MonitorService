using System;
using System.Collections.Generic;
using System.Text;

namespace NUTMonitor.Settings
{
    public class EsxiSettings
    {
        public EsxiEndpointSettings Endpoint { get; set; }

        public EsxiVmDescriptorSetting[] VirtualMachines { get; set; }
    }
}
