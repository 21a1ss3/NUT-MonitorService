using System;
using System.Collections.Generic;
using System.Text;

namespace NUTMonitor.Settings
{
    public static class SettingsHolder
    {
        static SettingsHolder()
        {
            Settings = new Settings();
        }

        public static Settings Settings { get; private set; }
    }
}
