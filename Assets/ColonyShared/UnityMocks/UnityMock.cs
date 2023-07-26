using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using SharedCode.Entities.GameObjectEntities;

namespace LocalServer.UnityMocks
{
    public static class UnityMock
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public static void MockUnityServices()
        {
            EntitytObjectsUnitySpawnService.SpawnService = new UnityServiceMock();
        }
    }
}
