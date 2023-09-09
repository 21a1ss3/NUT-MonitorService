using System;
using System.Collections.Generic;
using System.Text;

namespace NUTMonitor.Esxi.Vmodl
{
    internal class VmodlReaderFrame
    {
        //public VmodlNode CurrentNode { get; set; }
        public VmodlToken CurrentTokenType { get; set; }
        public VmodlReaderState State { get; set; } = VmodlReaderState.Initial;

        public string Buffer;
    }
}
