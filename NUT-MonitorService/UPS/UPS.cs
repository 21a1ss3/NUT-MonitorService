using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NUTMonitor.UPS
{
    public class UPS
    {
        public UPS(int upsId)
        {
            UpsId = upsId;

            _eventLogsRo[0] = new ReadOnlyCollection<LogEntry>(_eventLogs[0]);
            _eventLogsRo[1] = new ReadOnlyCollection<LogEntry>(_eventLogs[1]);

            LastTimePowerLoss = DateTime.MinValue;
            LastTimePowerRecovery = DateTime.MinValue;
        }

        private UPSMeasure _lastDBFixedMeasure = null;
        private List<UPSMeasure> _measuresToProcess = new List<UPSMeasure>();
        private ReadOnlyCollection<UPSMeasure> _measuresToProcessRo;
        private List<LogEntry>[] _eventLogs = new List<LogEntry>[2] { new List<LogEntry>(), new List<LogEntry>() };
        private ReadOnlyCollection<LogEntry>[] _eventLogsRo = new ReadOnlyCollection<LogEntry>[2];
        private volatile int _currentLog;
        private SemaphoreSlim _dbProcessSync = new SemaphoreSlim(0, 1);
        private AsyncReaderWriterLock _measuresLock = new AsyncReaderWriterLock();
        private bool _initialPowerSetup = true;


        public UPSMeasure LastDbFixedMeasure
        {
            get { return _lastDBFixedMeasure; }
            protected set { _lastDBFixedMeasure = value; }
        }



        public ReadOnlyCollection<UPSMeasure> MeasuresToProcess
        {
            get
            {
                if (_measuresToProcessRo == null)
                    _measuresToProcessRo = new ReadOnlyCollection<UPSMeasure>(_measuresToProcess);

                return _measuresToProcessRo;
            }
        }

        public ReadOnlyCollection<LogEntry> LogCurrentBunch
        {
            get { return _eventLogsRo[_currentLog]; }
        }


        public UPSMeasure LastMeasure { get; private set; }
        public int UpsId { get; private set; }
        public Guid MonitorSessionId { get; private set; }
        public DateTime LastTimePowerLoss { get; private set; }
        public DateTime LastTimePowerRecovery { get; private set; }
        public bool IsOnPower { get; private set; }

        internal SemaphoreSlim DbProcessEvent { get => _dbProcessSync; }


        internal void BeginNewMonitorSession()
        {
            MonitorSessionId = Guid.NewGuid();
            LastDbFixedMeasure = null;
        }

        private async Task _removeCompletedMeasures()
        {
            List<UPSMeasure> toRemove = new List<UPSMeasure>(_measuresToProcess.Count);

            using (await _measuresLock.ReaderLockAsync())
                foreach (var measure in _measuresToProcess)
                    if (measure.ProcessingStatus.IsFullyProcessed)
                        toRemove.Add(measure);

            using (await _measuresLock.WriterLockAsync())
                foreach (var measure in toRemove)
                    _measuresToProcess.Remove(measure);
        }

        internal async Task MarkAsDBProcessed(UPSMeasure measure)
        {
            if (measure == null)
                throw new ArgumentNullException(nameof(measure));

            using (await _measuresLock.ReaderLockAsync())
            {
                int index = _measuresToProcess.IndexOf(measure);

                if (index < 0)
                    throw new InvalidOperationException("Current measure is not present on measure collection");

                _measuresToProcess[index].ProcessingStatus.FixedInDb = true;
                LastDbFixedMeasure = measure;
            }

            await _removeCompletedMeasures();
        }

        public async Task ProcessMeasure(UPSMeasure measure)
        {
            if (measure == null)
                throw new ArgumentNullException(nameof(measure));

            using (await _measuresLock.ReaderLockAsync())
            {
                _measuresToProcess.Add(measure);
                LastMeasure = measure;
            }

            _dbProcessSync.Release();

            if (measure.InputVoltage < 170)
            {
                if (IsOnPower || _initialPowerSetup)
                {
                    LastTimePowerLoss = measure.Timestamp;
                    IsOnPower = false;
                }
            }
            else
            {
                if (!IsOnPower || _initialPowerSetup)
                {
                    LastTimePowerRecovery = LastTimePowerRecovery;
                    IsOnPower = true;
                }
            }
        }

        private void _toogleLogs()
        {
            List<LogEntry> currentBanch = _eventLogs[_currentLog];
            _currentLog = (_currentLog + 1) % 2;
            Task.Run(() => _dumpToDisk(currentBanch));

        }

        private void _dumpToDisk(List<LogEntry> currentLogs)
        {
            string fileName = string.Empty;
            try
            {
                int numPref = 0;

                LogContainer container = new LogContainer();

                container.Logs = Newtonsoft.Json.JsonConvert.SerializeObject(currentLogs, new Newtonsoft.Json.JsonSerializerSettings
                {
                    TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,                    
                }) ;
                container.RefreshHashes();

                do
                    fileName = Path.Combine(Settings.SettingsHolder.Settings.Logging.UnhandledLogPath, $"dump_logs_{DateTime.UtcNow:yyyy-MM-dd-HH-mm-ss}_{numPref++:D2}.json");
                while (File.Exists(fileName));

                using (StreamWriter logWriter = new StreamWriter(File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Read)))
                {
                    logWriter.AutoFlush = true;

                    logWriter.Write(Newtonsoft.Json.JsonConvert.SerializeObject(container));

                    logWriter.Close();
                }

            }
            catch (Exception ex)
            {
                lock (_eventLogs)                
                    _eventLogs[_currentLog].Add(new LogDumpErrorLogEntry() { Exception = ex, FileName = fileName });                
            }
            finally
            {
                currentLogs.Clear();
            }
        }

        public void RegisterTaskFail<T>(T source, Exception exception) where T : Tasks.MonitorTask
        {
            lock (_eventLogs)
            {
                if (Settings.SettingsHolder.Settings.Logging.InMemoryLimit == _eventLogs[_currentLog].Count)
                    _toogleLogs();

                TaskFailLogEntry failLogEntry = new TaskFailLogEntry();

                failLogEntry.TaskType = source.GetType().FullName;
                failLogEntry.Exception = exception;

                _eventLogs[_currentLog].Add(failLogEntry);
            }
        }
    }
}
