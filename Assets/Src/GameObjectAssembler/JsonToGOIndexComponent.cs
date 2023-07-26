using Assets.Src.GameObjectAssembler.Res;
using ResourcesSystem.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Src.GameObjectAssembler
{
    public class JsonToGOIndexComponent : MonoBehaviour
    {
        [Serializable]
        public struct ComponentMapping
        {
            public ulong ResourceID;
            public ushort Line;
            public ushort Col;
            public ushort ProtoIndex;

            public Component Target;

            public ComponentMapping(IComponentDef resource, Component target)
            {
                var id = GameResourcesHolder.Instance.GetID(resource);
                ResourceID = id.RootID();
                Col = (ushort)id.Col;
                Line = (ushort)id.Line;
                ProtoIndex = (ushort)id.ProtoIndex;

                Target = target;
            }

            public IComponentDef Def
            {
                get
                {
                    var id = GameResourcesHolder.Instance.NetIDs.GetID(ResourceID, Line, Col, ProtoIndex);
                    return GameResourcesHolder.Instance.LoadResource<IComponentDef>(id);
                }
            }
        }

        private static readonly IReadOnlyDictionary<IComponentDef, Component> Empty = new Dictionary<IComponentDef, Component>();

        [SerializeField]
        private ComponentMapping[] _mappings = Array.Empty<ComponentMapping>();

        private IReadOnlyDictionary<IComponentDef, Component> _mappingsMap;
        public IReadOnlyDictionary<IComponentDef, Component> Mappings
        {
            get
            {
                return _mappings.Length > 0 ? _mappingsMap ?? (_mappingsMap = _mappings.ToDictionary(x=>x.Def, x=>x.Target)) : Empty;
            }
            set
            {
                _mappingsMap = value;
                if(_mappingsMap != null && false) // Heavy load, debug-only
                {
                    foreach(var item in _mappingsMap)
                    {
                        if (!gameObject.GetComponentsInChildren(item.Value.GetType()).Any(v => v == item.Value))
                            throw new InvalidOperationException($"Out-of-object Ref to component {item.Value}");
                    }
                }
                _mappings = _mappingsMap != null ? _mappingsMap.Select(v => new ComponentMapping(v.Key, v.Value)).ToArray() : Array.Empty<ComponentMapping>();
            }
        }

    }
}
