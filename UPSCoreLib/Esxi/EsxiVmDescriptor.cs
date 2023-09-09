using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace NUTMonitor.Esxi
{
    public class EsxiVmDescriptor : IEquatable<EsxiVmDescriptor>
    {
        public string VMName { get; set; }
        public EsxiVmDescriptor[] DependsOn { get; set; }

        public int EsxiId { get; set; }
        public string Datastore { get; set; }
        public string Path { get; set; }
        public EsxiVmPowerState PowerState { get; set; }

        public EsxiVmPowerAction PowerAction { get; set; } = EsxiVmPowerAction.None;


        #region "Equals implementation"
        public override bool Equals(object obj)
        {
            return Equals(obj as EsxiVmDescriptor);
        }

        public bool Equals([AllowNull] EsxiVmDescriptor other)
        {
            return other != null &&
                   VMName == other.VMName;
        }

        public override int GetHashCode()
        {
            return VMName.GetHashCode();
        }

        public static bool operator ==(EsxiVmDescriptor left, EsxiVmDescriptor right)
        {
            return EqualityComparer<EsxiVmDescriptor>.Default.Equals(left, right);
        }

        public static bool operator !=(EsxiVmDescriptor left, EsxiVmDescriptor right)
        {
            return !(left == right);
        }
        #endregion
    }


}
