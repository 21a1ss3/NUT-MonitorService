using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NUTMonitor.UPS
{
    public class UPSMeasureStatus
    {
        public enum FailureKind
        {
            DB
        }

        public class FailureDescriptor
        {
            //For serialization
            private FailureDescriptor()
            {

            }

            internal FailureDescriptor(FailureKind kind, Exception exception)
            {
                Kind = kind;
                Exception = exception;
            }


            public FailureKind Kind { get; private set; }
            public Exception Exception { get; private set; }
        }

        public UPSMeasureStatus()
        {
            _failuresRO = new ReadOnlyCollection<FailureDescriptor>(_failures);
        }

        private List<FailureDescriptor> _failures = new List<FailureDescriptor>();
        private ReadOnlyCollection<FailureDescriptor> _failuresRO;
        public IList<FailureDescriptor> Failures => _failuresRO;

        public bool FixedInDb { get; set; }
        public bool IsDbFailed { get; set; }



        public int DbProcessingAttempts { get; set; }

        public bool IsFullyProcessed { get; private set; }

        internal void ProcessDBFailure(Exception exception)
        {
            _failures.Add(new FailureDescriptor(FailureKind.DB, exception));
            IsDbFailed = true;
        }
    }
}
