using System;
using System.Collections.Generic;
using System.Text;

namespace NUTMonitor.Esxi
{
    public class EsxiCredentialConnectionParameters : EsxiConnectionParameters
    {
        public EsxiCredentialConnectionParameters(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; private set; }
        public string Password { get; private set; }
    }
}
