//Made by Vitaly Bulanenkov
//while working at and for Enplex Games
//in 2017
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Utilities
{

    [InitializeOnLoad]
    public static class RespectTerrainWithOffset
    {
        static RespectTerrainWithOffset()
        {
            EditorApplication.update += OnEditorUpdate;
        }
        [MenuItem("Level Design/Respect Terrain with offset &T")]
        static void ChangeMode()
        {
            _enabled = !_enabled;
            if (_enabled)
            {
                SelectOffsets();
            }
            Menu.SetChecked("Level Design/Respect Terrain with offset &T", _enabled);
        }
        private static Dictionary<GameObject, float> _lastGOs = new Dictionary<GameObject, float>();
        private static bool _enabled = false;

        static void SelectOffsets()
        {
            _lastGOs.Clear();
            var maskInt = LayerMask.GetMask("Terrain");
            RaycastHit hit;
            foreach (var go in Selection.gameObjects)
            {
                var offset = 0f;
                var goTransform = go.transform;

                if (Physics.Raycast(new Ray(goTransform.position, Vector3.up), out hit, 2000f, maskInt))
                    offset = goTransform.position.y - hit.point.y;
                else if (Physics.Raycast(new Ray(goTransform.position, Vector3.down), out hit, 2000f, maskInt))
                    offset = goTransform.position.y - hit.point.y;
                _lastGOs.Add(go, offset);
            }
        }
        static void OnEditorUpdate()
        {
            if(_enabled == false)
                return;
            if (Selection.gameObjects.Length != _lastGOs.Count)
            {
                SelectOffsets();
                return;
            }
            foreach (var gameObject in Selection.gameObjects)
            {
                if (gameObject.GetComponent<Terrain>() != null)
                {
                    return;
                }
                if (!_lastGOs.ContainsKey(gameObject))
                {
                    SelectOffsets();
                    return;
                }
            }
            var maskInt = LayerMask.GetMask("Terrain");
            RaycastHit hit;
            foreach (var go in Selection.gameObjects)
            {
                var goTransform = go.transform;
                if (Physics.Raycast(new Ray(goTransform.position, Vector3.up), out hit, 2000f, maskInt))
                    goTransform.position = hit.point + new Vector3(0, _lastGOs[go], 0);
                else if (Physics.Raycast(new Ray(goTransform.position, Vector3.down), out hit, 2000f, maskInt))
                    goTransform.position = hit.point + new Vector3(0, _lastGOs[go], 0);

            }
            
        }
        
    }

}