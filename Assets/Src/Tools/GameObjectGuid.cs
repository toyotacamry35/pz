using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Src.Tools
{
 /*
    /// <summary>
    /// Уникальный идентификатор объекта в префабе. Предназначен для идентификации этого же объекта на инстансе того же префаба.
    /// TODO: штука опасная, так при копировании объекта/префаба будет скопирован и Guid, а проверок на дубликаты нет.
    /// </summary>
    public class GameObjectGuid : MonoBehaviour
    {
        [SerializeField] private SerializableGuid _guid = SerializableGuid.NewGuid();

        public SerializableGuid Guid => _guid;

        private void Reset()
        {
            _guid = SerializableGuid.NewGuid();
        }
    }


    public static partial class GameObjectExtension
    {
        public static SerializableGuid Guid(this GameObject obj)
        {
            var g = obj.GetComponent<GameObjectGuid>();
            if(!g)
                throw new Exception($"GameObjectGuid component not exists on {obj.transform.FullName()}");
            if(!g.Guid.IsValid)
                throw new Exception($"Guid is empty on {obj.transform.FullName()}");
            return g.Guid;
        }
        
        public static SerializableGuid Guid(this Component obj)
        {
            var g = (obj as GameObjectGuid) ?? obj.GetComponent<GameObjectGuid>();
            if(!g)
                throw new Exception($"GameObjectGuid component not exists on {obj.transform.FullName()}");
            if(!g.Guid.IsValid)
                throw new Exception($"Guid is empty on {obj.transform.FullName()}");
            return g.Guid;
        }

        #if UNITY_EDITOR
        public static SerializableGuid CreateGuid(this GameObject obj)
        {
            var g = obj.GetComponent<GameObjectGuid>();
            if (!g)
            {
                g = obj.AddComponent<GameObjectGuid>();
                EditorUtility.SetDirty(obj);
            }
            if(!g.Guid.IsValid)
            {
                GameObject.DestroyImmediate(g, true);
                g = obj.AddComponent<GameObjectGuid>();
                EditorUtility.SetDirty(obj);
            }
            return g.Guid;
        }
        #endif
    }
    */
 
}