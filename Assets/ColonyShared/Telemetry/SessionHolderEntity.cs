using System;
using System.Threading.Tasks;

namespace GeneratedCode.DeltaObjects
{
    public partial class SessionHolderEntity : ISessionHolderEntityImplementRemoteMethods
    {
        public ValueTask RegisterImpl(Guid guid, Guid session)
        {
            SessionsByGuid[guid] = session;
            return new ValueTask();
        }
        public ValueTask UnregisterImpl(Guid guid)
        {
            SessionsByGuid.Remove(guid);
            return new ValueTask();
        }
    }
}
