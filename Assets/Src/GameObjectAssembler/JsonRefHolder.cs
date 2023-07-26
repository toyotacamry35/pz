using System;
using Assets.Src.GameObjectAssembler.Res;
using ResourcesSystem.Loader;
using UnityEngine;

namespace Assets.Src.GameObjectAssembler
{
    [DisallowMultipleComponent]
    public class JsonRefHolder : MonoBehaviour
    {
        public bool InitializedByItself { get; set; } = true;
        //public string Ref;

        private UnityGameObjectDef _def;

        public UnityGameObjectDef Definition
        {
            get
            {
                if (!enabled)
                    return null;

                //if (_def == null && !String.IsNullOrWhiteSpace(Ref))
                //    _def = GameResourcesHolder.Instance.LoadResource<UnityGameObjectDef>(Ref);
                return _def;
            }
            set { _def = value; }
        }
    }
}
