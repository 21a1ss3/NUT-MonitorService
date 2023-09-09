using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SSH = Renci.SshNet;

namespace NUTMonitor.Tasks
{
    public class PowerStateMonitorTask : TimeredMonitorTask
    {
        public PowerStateMonitorTask(UPS.UPS upsEngine) : base(upsEngine)
        {
            Task.Run(() => _buildVmList());
        }

        private DateTime _lastProcessedLossTime = DateTime.MinValue;
        private bool _hasFirstTriggered = false;
        private bool _secondShortLoss = false;
        private ManualResetEventSlim _sequenceBuild = new ManualResetEventSlim(false);
        private object _powerActionLock = new object();
        private List<Esxi.EsxiVmDescriptor> _vmPowerList = new List<Esxi.EsxiVmDescriptor>();
        private SSH.SshClient _sshClient;

        private TimeSpan _ordinaryPollingInterval;

        public TimeSpan OrdinaryPollingInterval
        {
            get { return _ordinaryPollingInterval; }
            set { _ordinaryPollingInterval = value; }
        }

        protected override Task OnLaunch()
        {
            Interval = OrdinaryPollingInterval;

            return Task.CompletedTask;
        }

        protected override Task Tick()
        {
            if (Interval != OrdinaryPollingInterval)
                Interval = OrdinaryPollingInterval;

            if (!UpsEngine.IsOnPower)
            {
                if (_hasFirstTriggered)
                {
                    if ((UpsEngine.LastTimePowerLoss - _lastProcessedLossTime) == TimeSpan.Zero)
                        _shutdownActions();
                    else
                        _secondShortLoss = true;

                    _hasFirstTriggered = false;
                }
                else if (_secondShortLoss)
                {
                    if ((UpsEngine.LastTimePowerLoss - _lastProcessedLossTime) >= TimeSpan.FromMinutes(5))
                        _shutdownActions();

                    _secondShortLoss = false;
                }
                else
                    _hasFirstTriggered = true;


                _lastProcessedLossTime = UpsEngine.LastTimePowerLoss;
                TimeSpan newPollInt = TimeSpan.FromMinutes(1) - (DateTime.UtcNow - _lastProcessedLossTime);

                if (newPollInt < TimeSpan.Zero)
                    newPollInt = TimeSpan.Zero;

                Interval = newPollInt.Add(TimeSpan.FromSeconds(10));
            }

            return Task.CompletedTask;
        }

        private void _buildVmList()
        {
            _vmPowerList.Clear();
            while (_vmPowerList.Count < Settings.SettingsHolder.Settings.Esxi.VirtualMachines.Length)
            {
                int prevCount = _vmPowerList.Count;
                foreach (var vmDesc in Settings.SettingsHolder.Settings.Esxi.VirtualMachines)
                {
                    bool toAdd = true;
                    for (int i = 0; (i < vmDesc.DependsOn.Length) && toAdd; i++)
                        if (!_vmPowerList.Contains(vmDesc.DependsOn[i]))
                            toAdd = false;

                    if (toAdd)
                        _vmPowerList.Add(vmDesc);
                }

                if (_vmPowerList.Count == 0)
                    throw new Exception("Virtual machine list contain infitive loop or external references");

                if (prevCount == _vmPowerList.Count)
                    throw new Exception("Subset of virtual machine list contain infitive loop or external references");
            }

            _sequenceBuild.Set();
        }

        private void _shutdownActions()
        {
            _sequenceBuild.Wait();

            lock (_powerActionLock)
            {
                
            }
        }
    }
}
