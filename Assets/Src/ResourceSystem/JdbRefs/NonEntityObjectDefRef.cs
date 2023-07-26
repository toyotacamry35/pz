using System;
using SharedCode.Entities.GameObjectEntities;
using UnityEngine;

namespace Assets.Src.ResourceSystem
{
    [Serializable]
    public class NonEntityObjectDefRef : JdbRef<NonEntityObjectDef>
    {
        public ScriptableObject Metadata { get => _metadata; set => _metadata = (JdbMetadata)value; }
    }
}