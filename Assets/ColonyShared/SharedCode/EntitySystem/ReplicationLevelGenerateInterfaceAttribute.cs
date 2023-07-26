using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCode.EntitySystem
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ReplicationLevelGenerateInterfaceAttribute: Attribute
    {
    }
}
