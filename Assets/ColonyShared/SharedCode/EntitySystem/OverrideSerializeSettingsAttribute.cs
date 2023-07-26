using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCode.EntitySystem
{
    public class OverrideSerializeSettingsAttribute: Attribute
    {
        public bool DynamicType { get; set; }
        public bool AsReference { get; set; }
        public bool OverwriteList { get; set; }
    }
}
