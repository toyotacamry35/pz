using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCode.EntitySystem
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Event)]
    public class ReplicationLevelAttribute: Attribute
    {
        public ReplicationLevel ReplicationLevel { get; }

        public ReplicationLevelAttribute(ReplicationLevel replicationLevel)
        {
            ReplicationLevel = replicationLevel;
        }
    }
}
