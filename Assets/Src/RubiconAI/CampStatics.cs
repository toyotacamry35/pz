using Assets.Src.ResourceSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Src.RubiconAI.Groups
{
    public class CampStatics : MonoBehaviour
    {
        public string Name;
        // радиус, в котором моб будет видеть эти точки интереса. Радиус рассчитывается относительно точки спауна, а не самого моба! 
        //.. (https://centre.atlassian.net/wiki/spaces/COL/pages/904101907/POI)
        public float Radius = 100;
        public List<GameObject> Links = new List<GameObject>();
        public List<GameObject> FoundLinks = new List<GameObject>();
        public GameObject PrefabPOI;

        void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (Name != null)
                foreach (var link in Links)
                {
                    if (link != null)
                        UnityEditor.Handles.Label(
                            link.transform.position,
                            Name);
                }
#endif
        }
    }

}