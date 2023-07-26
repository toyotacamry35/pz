using SharedCode.EntitySystem;
using System.Threading;

namespace GeneratedCode.Manual.Repositories
{
    public static class RpcCurrentObject
    {
        private static readonly AsyncLocal<IDeltaObject> _currentObj = new AsyncLocal<IDeltaObject>();

        public static IDeltaObject ThisObj
        {
            get => _currentObj.Value;
            set => _currentObj.Value = value;
        }
    }
}
