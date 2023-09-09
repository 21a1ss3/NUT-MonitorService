using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NUTMonitor.DB.UpsStatus
{
    public class UpsStatus
    {
        [Key]
        public int StatusID { get; set; }

        public string StatusName { get; set; }

        public bool IsManuallySetuped { get; set; }
        public bool IsConnectedToAC { get; set; }

        public ICollection<UpsMesurement> UpsMesurements { get; set; }
    }
}
