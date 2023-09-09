using NUTMonitor.DB.UpsStatus;
using System;
using System.Collections.Generic;
using System.Text;

namespace NUTMonitor.Sensetive.DB
{
    internal class UpsMonDbconnectionString : IDbSensetiveProvider
    {
        public string GetConnectionString()
        {
            return @"User Id=;Password=;Data Source=homedb_high;Connection Timeout=120";
        }
    }
}
