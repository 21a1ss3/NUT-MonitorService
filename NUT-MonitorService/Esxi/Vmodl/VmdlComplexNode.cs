using System;
using System.Collections.Generic;
using System.Text;

namespace NUTMonitor.Esxi.Vmodl
{
    public class VmdlComplexNode : VmodlNode
    {
        public Dictionary<string, VmodlNode> Properties { get; set; }
    }
}
