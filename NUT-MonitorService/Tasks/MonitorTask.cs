using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NUTMonitor.Tasks
{
    public abstract class MonitorTask
    {
        public MonitorTask(UPS.UPS upsEngine)
        {
            UpsEngine = upsEngine ?? throw new ArgumentNullException(nameof(upsEngine));

            RelaunchOnFailureLimit = -1;
            RelaunchOnFailureAttempts = 0;

            _workerTask = new Task(() =>
            {
                Task internalTask = InternalLaunch();

                internalTask.GetAwaiter().GetResult();
            });

            _onFailureAction = _workerTask.ContinueWith((prevTask) =>
            {
                IsRun = false;
                RunningException = null;
                if (_workerTask.IsFaulted)
                {
                    RunningException = _workerTask.Exception;
                    UpsEngine.RegisterTaskFail(this, RunningException);
                }

                if (RelaunchOnFailureTimeout >= TimeSpan.Zero)
                {
                    int attempts = Interlocked.Increment(ref _autoRelaunchOnFailureAttempts);

                    if ((RelaunchOnFailureLimit != -1) && (_autoRelaunchOnFailureAttempts > RelaunchOnFailureLimit))
                    {
                        return;
                    }

                    Task.Run(async () =>
                    {
                        await Task.Delay(RelaunchOnFailureTimeout);
                        Run();
                    });
                }
            }, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.PreferFairness); //Check TaskContinuationOptions.NotOnRanToCompletion
        }

        protected abstract Task InternalLaunch();

        private Task _workerTask;
        private Task _onFailureAction;
        private int _autoRelaunchOnFailureAttempts;

        public TimeSpan RelaunchOnFailureTimeout { get; set; }
        public int RelaunchOnFailureLimit { get; set; }
        public int RelaunchOnFailureAttempts
        {
            get => _autoRelaunchOnFailureAttempts;
            private set => _autoRelaunchOnFailureAttempts = value;
        }
        public Exception LaunchException { get; private set; }
        public Exception RunningException { get; private set; }
        public bool IsRun { get; private set; }
        public UPS.UPS UpsEngine { get; protected set; }

        public void Run()
        {
            LaunchException = null;
            IsRun = true;
            try
            {
                _workerTask.Start();
            }
            catch (Exception ex)
            {
                IsRun = false;
                LaunchException = ex;
                UpsEngine.RegisterTaskFail(this, LaunchException);
            }
        }
    }
}
