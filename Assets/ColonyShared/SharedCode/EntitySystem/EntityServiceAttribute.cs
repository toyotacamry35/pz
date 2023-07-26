using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCode.Cloud;

namespace SharedCode.EntitySystem
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public class EntityServiceAttribute: Attribute
    {
        private CloudNodeType _replicateToNodeType;

        private CloudNodeType _addedByDefaultToNodeType;

        public bool ReplicateTo(CloudNodeType nodeType)
        {
            return (_replicateToNodeType & nodeType) == nodeType;
        }

        public bool AddedByDefaultTo(CloudNodeType nodeType)
        {
            return (_addedByDefaultToNodeType & nodeType) == nodeType;
        }
        
        public EntityServiceAttribute():this(CloudNodeType.None, CloudNodeType.None)
        {
        }

        public EntityServiceAttribute(CloudNodeType replicateToNodeType = CloudNodeType.None, CloudNodeType addedByDefaultToNodeType = CloudNodeType.None)
        {
            _replicateToNodeType = replicateToNodeType;
            _addedByDefaultToNodeType = addedByDefaultToNodeType;
        }
    }
}
