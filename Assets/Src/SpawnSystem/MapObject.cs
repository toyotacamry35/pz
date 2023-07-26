using Assets.Src.ResourceSystem;
using Assets.Src.Tools;
using GeneratedCode.Repositories;
using SharedCode.Repositories;
using SharedCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Src.SpawnSystem
{
    [ExecuteInEditMode]
    public class MapObject : MonoBehaviour
    {
        public JdbMetadata ObjectToSpawn;
        public JdbMetadata JdbLocator;
        [SerializeField] public string _guid;
        public Guid ObjectGuid => Guid.Parse(_guid);
        public float TimeToRespawn;
        public int TypeId => ObjectToSpawn == null ? 0 : ReplicaTypeRegistry.GetIdByType(DefToType.GetEntityType(ObjectToSpawn.Get().GetType()));
        private void OnEnable()
        {
            if (_guid.IsNullOrWhitespace())
            {
#if UNITY_EDITOR
                var obj = new UnityEditor.SerializedObject(this);
                obj.FindProperty(nameof(_guid)).stringValue = Guid.NewGuid().ToString();
                obj.ApplyModifiedProperties();
#endif
            }
        }

    }


}
