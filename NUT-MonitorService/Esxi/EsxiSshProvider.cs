using System;
using System.Collections.Generic;
using System.Text;

namespace NUTMonitor.Esxi
{
    internal class EsxiSshProvider : EsxiCliProvider
    {
        protected override bool CanHandle(Uri path, EsxiConnectionParameters parameters) => path?.Scheme.ToLower() == "ssh" && (parameters is EsxiCredentialConnectionParameters);




        protected override void ProcessOpenResource()
        {
            throw new NotImplementedException();
        }

        protected override void ProcessCloseResource()
        {
            throw new NotImplementedException();
        }

        protected override string ExecuteCommand(string command)
        {
            throw new NotImplementedException();
        }
    }
}
