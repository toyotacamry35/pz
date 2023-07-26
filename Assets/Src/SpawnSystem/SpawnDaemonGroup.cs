using Assets.Src.ResourceSystem;
using SharedCode.Entities.GameObjectEntities;
using System;
using UnityEngine;
using Assets.Src.Tools;
using System.Linq;
using ResourcesSystem.Loader;

namespace Assets.Src.SpawnSystem
{
    [ExecuteInEditMode]
    public class SpawnDaemonGroup : MapObject
    {
        public JdbMetadata SpawnDaemonGoals;
        public JdbMetadata TemplatesMap;
        [SerializeField] public string _spawnDaemonName;
        public Guid SpawnDaemonGuid => Guid.Parse(_guid);

        public string Filter;
        public string[] Filters;


        private void OnEnable()
        {
#if UNITY_EDITOR
            _res = null;
#endif
            if (_guid.IsNullOrWhitespace())
            {

#if UNITY_EDITOR
                var obj = new UnityEditor.SerializedObject(this);
                obj.FindProperty(nameof(_guid)).stringValue = Guid.NewGuid().ToString();
                obj.ApplyModifiedProperties();
#endif
            }
        }
        private void OnValidate()
        {
            if (_spawnDaemonName.IsNullOrWhitespace())
                _spawnDaemonName = "SpawnDaemon" + new System.Random().Next(9999999);
        }
#if UNITY_EDITOR
        static GameResources _res;
        private void OnDrawGizmosSelected()
        {
            var sceneName = this.gameObject.scene.name;
            var refPath = $"/SpawnSystemData/{sceneName}/{sceneName}";
            if (_res == null)
                _res = EditorGameResourcesForMonoBehaviours.GetNew();
            var sceneChunk = _res.TryLoadResource<SceneChunkDef>(refPath);
            if(sceneChunk != null)
            {

                var daemon = sceneChunk.Entities.Where(x=>x.Target.SpawnDaemonSceneDef.Target != null).FirstOrDefault(x => x.Target.SpawnDaemonSceneDef.Target.SpawnDaemonId == SpawnDaemonGuid);
                if (daemon != null)

                {
                    foreach (var point in daemon.Target.SpawnDaemonSceneDef.Target.MobsSpawnPoints)
                    {
                        Gizmos.DrawWireSphere(new Vector3(point.x, point.y, point.z) + transform.position, 1f);
                    }
                }
            }
        }
#endif
    }
}
