using System;
using System.Collections.Generic;
using System.Text;

namespace NUTMonitor.DB.UpsStatus
{
    internal interface IDbSensetiveProvider
    {
        string GetConnectionString();
    }
}
