using System;
using System.Collections.Generic;
using System.Text;

namespace NUTMonitor.Esxi.Vmodl
{
    public class VmodlSimpleNode : VmodlNode
    {
        public object Value { get; set; }

        private string UnescapedValue { get; set; }
        private string EscapedValue { get; set; }
    }
}
