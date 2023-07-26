using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Src.Aspects.Impl
{
    public class SpawnGroup : MonoBehaviour
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static Dictionary<Guid, SpawnGroup> GroupPerGuid = new Dictionary<Guid, SpawnGroup>();
        Guid _daemonGuid;
        
    }
}
