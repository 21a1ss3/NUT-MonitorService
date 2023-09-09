using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NUTMonitor.UPS
{
    public static class UPSStatuses
    {
        private static object _synObj = new object();

        private static void _initStatuses()
        {
            _statuses = new List<UPSStatus>();
            _statusesRO = new ReadOnlyCollection<UPSStatus>(_statuses);

            _statuses.Add(new UPSStatus() { StringName = "OL", IsConnectedToAC = true, IsWellKnown = true });
            _statuses.Add(new UPSStatus() { StringName = "OB", IsConnectedToAC = false, IsWellKnown = true });
            _statuses.Add(new UPSStatus() { StringName = "OL BYPASS", IsConnectedToAC = true, IsWellKnown = true });
            _statuses.Add(new UPSStatus() { StringName = "OL TRIM", IsConnectedToAC = true, IsWellKnown = true });
            _statuses.Add(new UPSStatus() { StringName = "OB LB", IsConnectedToAC = false, IsWellKnown = true });
        }

        private static List<UPSStatus> _statuses;
        private static ReadOnlyCollection<UPSStatus> _statusesRO;
        public static ReadOnlyCollection<UPSStatus> Statuses
        {
            get
            {
                lock (_synObj)
                    if (_statuses == null)
                        _initStatuses();

                return _statusesRO;
            }
        }

    }
}
