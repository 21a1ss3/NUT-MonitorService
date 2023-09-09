using System;
using System.Collections.Generic;
using System.Text;

namespace NUTMonitor.Settings
{
    public class EsxiEndpointSettings
    {
        public Uri ResourseAddress { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
    }
}
