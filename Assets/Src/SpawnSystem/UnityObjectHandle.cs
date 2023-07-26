using Assets.Src.RubiconAI;
using SharedCode.Cloud;
using SharedCode.Entities.GameObjectEntities;
using UnityEngine;

namespace Assets.Src.SpawnSystem
{
    class UnityObjectHandle : IUnityObjectHandle
    {
        public GameObject Obj;
        public EntityGameObject Ego;
        public ISpatialLegionary Leg;
        public bool Instantiated = false;
        public CloudNodeType Repos;
        public bool HasClientAuthority = false;
        public bool HasServerAuthority = false;

        public override string ToString()
        {
            return $"Obj = { (Obj == null ? "null" : Obj.name) }, Ego = { (Ego == null ? "null" : Ego.name) }, Instantiated = {Instantiated}, Repos = {Repos}";
        }
    }
}
