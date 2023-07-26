using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

namespace DeepSky.Haze
{
    /// <summary>
    /// The GameObject menu items for creating gameobjects with the Controller and Zone components.
    /// </summary>
    public class DS_HazeMenuItems
    {
        [MenuItem("GameObject/DeepSky Haze/Controller", false, 51)]
        public static void CreateDeepSkyHazeController()
        {
            DS_HazeCore core = DS_HazeCore.Instance;
            GameObject cont;
            if (core == null)
            {
                cont = new GameObject("DS_HazeController", typeof(DS_HazeCore));
                cont.transform.position = new Vector3(0, 0, 0);
            }
            else
            {
                cont = core.gameObject;
                Debug.LogWarning("DeepSky::CreateDeepSkyHazeController - there is already a Haze Controller in this scene!");
            }

            Selection.objects = new Object[] { cont };
        }

        [MenuItem("GameObject/DeepSky Haze/Zone", false, 52)]
        public static void CreateDeepSkyHazeZone()
        {
            GameObject zone = new GameObject("DS_HazeZone", typeof(DS_HazeZone));
            zone.transform.localScale = new Vector3(100, 50, 100);

            DS_HazeCore core = DS_HazeCore.Instance;
            if (core != null)
            {
                zone.transform.SetParent(core.transform);
            }

            Selection.objects = new Object[] { zone };
        }
    }
}