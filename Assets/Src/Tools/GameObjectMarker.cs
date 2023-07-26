using System;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.ResourceSystem;
using ResourceSystem.Aspects.Misc;
using UnityEngine;

namespace Src.Tools
{
    public class GameObjectMarker : MonoBehaviour
    {
        [SerializeField] private GameObjectMarkerRef _markerRef;

        public GameObjectMarkerRef MarkerRef => _markerRef;
        public GameObjectMarkerDef MarkerDef => _markerRef.Target;
#if UNITY_EDITOR
        public EditorBaseResourceWrapper<GameObjectMarkerDef> EDITOR_MarkerDef => _markerRef.EDITOR_Target;
#endif        
    }
    
    [Serializable]
    public class GameObjectMarkerRef : JdbRef<GameObjectMarkerDef> {}
    
    
    public static partial class GameObjectExtension
    {
        public static GameObjectMarkerDef GetMarker(this GameObject obj)
        {
            return GetMarker(obj.GetComponent<GameObjectMarker>());
        }
        
        public static GameObjectMarkerDef GetMarker(this Component obj)
        {
            var g = (obj as GameObjectMarker);
            if (!g)
            {
                if (!obj)
                    throw new Exception($"obj is null in {nameof(GameObjectExtension)}.{nameof(GetMarker)}");
                g = obj.GetComponent<GameObjectMarker>();
            }
            if(!g)
                throw new Exception($"{nameof(GameObjectMarker)} component not exists on {obj.transform.FullName()}");
            if(g.MarkerDef == null)
                throw new Exception($"Marker is empty on {obj.transform.FullName()}");
            return g.MarkerDef;
        }

        public static GameObjectMarkerDef TryGetMarker(this Component obj)
        {
            var g = obj as GameObjectMarker;
            if (!g)
            {
                if (!obj)
                    return null;
                g = obj.GetComponent<GameObjectMarker>();
            }
            return g ? g.MarkerDef : null;
        }
        
        public static GameObjectMarkerRef GetMarkerRef(this GameObject obj)
        {
            return GetMarkerRef(obj.GetComponent<GameObjectMarker>());
        }
        
        public static GameObjectMarkerRef GetMarkerRef(this Component obj)
        {
            var g = (obj as GameObjectMarker) ?? obj.GetComponent<GameObjectMarker>();
            if(!g)
                throw new Exception($"{nameof(GameObjectMarker)} component not exists on {obj.transform.FullName()}");
            if(g.MarkerDef == null)
                throw new Exception($"Marker is empty on {obj.transform.FullName()}");
            return g.MarkerRef;
        }
        
        public static GameObjectMarkerRef TryGetMarkerRef(this Component obj)
        {
            var g = (obj as GameObjectMarker) ?? obj.GetComponent<GameObjectMarker>();
            return !g ? null : g.MarkerRef;
        }
    }
}
