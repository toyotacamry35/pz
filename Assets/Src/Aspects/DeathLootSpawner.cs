using Assets.Src.ResourceSystem;
using Assets.Src.SpawnSystem;
using Assets.Src.WorldSpace;
using SharedCode.Aspects.Item.Templates;
using System;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using UnityEngine;

using SharedQuaternion = SharedCode.Utils.Quaternion;
using SharedVector3 = SharedCode.Utils.Vector3;
using Assets.ColonyShared.SharedCode.Entities.Service;

namespace Assets.Src.Aspects
{
    class DeathLootSpawner : EntityGameObjectComponent
    {

        //=== Protected =======================================================

        protected override void GotServer()
        {
            SubscribeMortalServer(true);
        }

        protected override void LostServer()
        {
            SubscribeMortalServer(false);
        }

        // --- Privates: --------------------------------------------------------

        private void SubscribeMortalServer(bool subscribe)
        {
            var repo = ServerRepo;
            if (repo == null)
                return;


        }

    }

    [Serializable]
    public class WorldCorpseDefRef : JdbRef<WorldCorpseDef>
    {
    }
}