using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NUTMonitor.Tasks
{
    public abstract class TimeredMonitorTask : MonitorTask
    {
        public TimeredMonitorTask(UPS.UPS upsEngine)
            : base(upsEngine)
        {
            _runExceptionSync = new SemaphoreSlim(0, 1);

            _timer = new Timer((st) =>
            {
                try
                {
                    Tick().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                    _runException = ex;
                    _runExceptionSync.Release();
                }
            }, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        private Timer _timer;
        private TimeSpan _launchDelay = Timeout.InfiniteTimeSpan;
        private TimeSpan _interval = Timeout.InfiniteTimeSpan;
        private Exception _runException;
        private SemaphoreSlim _runExceptionSync;

        public TimeSpan LaunchDelay
        {
            get { return _launchDelay; }
            set
            {
                _launchDelay = value;
                if (IsRun)
                    _timer.Change(LaunchDelay, Interval);
            }
        }

        public TimeSpan Interval
        {
            get { return _interval; }
            set
            {
                _interval = value;
                if (IsRun)
                    _timer.Change(LaunchDelay, Interval);
            }
        }

        protected abstract Task Tick();
        protected abstract Task OnLaunch();


        protected sealed override async Task InternalLaunch()
        {
            await OnLaunch(); //???

            _timer.Change(LaunchDelay, Interval);

            await _runExceptionSync.WaitAsync();

            throw new Exception("Monitor task has failed. See inner exception", _runException);
        }
    }
}
