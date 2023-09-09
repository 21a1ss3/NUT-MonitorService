using System;

namespace NUTMonitor.Settings
{
    public class EsxiVmDescriptorSetting : Esxi.EsxiVmDescriptor
    {

        internal bool ShouldSerializeEsxiId() => false;
        internal bool ShouldSerializeDatastore() => false;
        internal bool ShouldSerializePath() => false;
        internal bool ShouldSerializePowerState() => false;

        internal bool ShouldSerializeEsxiVmPowerAction() => false;
    }
}
