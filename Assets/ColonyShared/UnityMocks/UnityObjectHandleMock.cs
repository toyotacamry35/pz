using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCode.Entities.GameObjectEntities;

namespace LocalServer.UnityMocks
{
    public class UnityObjectHandleMock: IUnityObjectHandle
    {
        public UnityObjectHandleMock(object gameObjectRef)
        {
            GameObjectRef = gameObjectRef;
        }

        public object GameObjectRef { get; }
    }
}
