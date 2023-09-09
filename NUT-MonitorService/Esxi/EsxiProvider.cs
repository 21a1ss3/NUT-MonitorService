using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NUTMonitor.Esxi
{
    public abstract class EsxiProvider
    {
        private static List<Type> _providers = new List<Type>();

        private static void _registerProvider<T>() where T : EsxiProvider, new()
        {
            if (!_providers.Contains(typeof(T)))
                _providers.Add(typeof(T));
        }

        public static void RegisterStandardProviders()
        {
            _registerProvider<EsxiSshProvider>();
        }

        public static EsxiProvider GetProvider(Uri path, EsxiConnectionParameters parameters)
        {
            foreach (var providerType in _providers)
            {
                EsxiProvider providerCandidate = (EsxiProvider)providerType.GetConstructor(new Type[0]).Invoke(null);

                if (providerCandidate.CanHandle(path, parameters))
                {
                    providerCandidate.ServicePoint = path;
                    providerCandidate.Parameters = parameters;

                    return providerCandidate;
                }
            }

            return null;
        }

        public Uri ServicePoint { get; private set; }
        protected EsxiConnectionParameters Parameters { get; private set; }

        private volatile int _activeSessions = 0;
        private object _syncObj = new object();

        protected abstract bool CanHandle(Uri path, EsxiConnectionParameters parameters);
        protected abstract void ProcessOpenResource();
        protected abstract void ProcessCloseResource();

        public abstract bool[] GetActualStatuses<T>(T vm) where T : IReadOnlyList<Esxi.EsxiVmDescriptor>, IReadOnlyCollection<Esxi.EsxiVmDescriptor>, IEnumerable<Esxi.EsxiVmDescriptor>;

        protected void Open()
        {
            if (_activeSessions == 0)
                lock (_syncObj)
                {
                    if (_activeSessions == 0)
                        ProcessOpenResource();
                }

            Interlocked.Increment(ref _activeSessions);
        }

        protected void Close()
        {
            lock (_syncObj)
            {
                if (Interlocked.Decrement(ref _activeSessions) == 0)
                    ProcessCloseResource();
            }

        }
    }
}
