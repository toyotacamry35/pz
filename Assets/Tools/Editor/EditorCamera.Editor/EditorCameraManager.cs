using Assets.TerrainBlend;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;
using ECamera = Assets.Tools.EditorCamera.EditorCamera;
using ECameraNotifier = Assets.Src.Camera.EditorCamera;

namespace Assets.Tools.Editor.EditorCamera.Editor
{
    public class EditorCameraManager
    {

        [MenuItem("Level Design/Create Editor Camera %#e")]
        public static void PlaceCamera()
        {
            ECamera oldCam = Object.FindObjectOfType<ECamera>();
            if (oldCam != null)
                GameObject.DestroyImmediate(oldCam.gameObject);

            GameObject editorCamera = new GameObject();
            editorCamera.name = "EditorCamera";
            editorCamera.AddComponent<ECamera>();
            editorCamera.hideFlags = HideFlags.DontSave;

            GameObject freeLookCamera = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/UniqueResources/GameCameraRig.prefab"));
            Transform cam = freeLookCamera.transform.GetComponent<Camera>().transform;
            cam.parent = null;
            Cinemachine.CinemachineBrain brain = cam.GetComponent<Cinemachine.CinemachineBrain>();
            if (brain != null)
                Object.DestroyImmediate(brain);
            cam.parent = editorCamera.transform;
            cam.localPosition = Vector3.zero;
            cam.localRotation = Quaternion.identity;
            cam.gameObject.AddComponent<ECameraNotifier>();
        }

        [MenuItem("Level Design/MeshBlending/Default Setup: materials, terrain affiliation")]
        public static void SetupObjects()
        {
            MeshBlending[] co = Object.FindObjectsOfType<MeshBlending>();
            for (int i = 0; i < co.Length; i++)
            {
                co[i].SetupDefault();
            }
        }

        [MenuItem("Level Design/MeshBlending/Setup materials only")]
        public static void SetupObjectsMaterials()
        {
            MeshBlending[] co = Object.FindObjectsOfType<MeshBlending>();
            for (int i = 0; i < co.Length; i++)
            {
                co[i].SetupWithoutCalculations();
            }
        }

        [MenuItem("Level Design/MeshBlending/Update renderers only")]
        public static void SetupObjectsRenderers()
        {
            MeshBlending[] co = Object.FindObjectsOfType<MeshBlending>();
            foreach (var c in co)
            {
                c.SetupRenderers();
            }
        }

        [MenuItem("Level Design/MeshBlending/Update renderers, terrain affiliation, materials and do Default Setup")]
        public static void SetupObjectsRenderersTerrainAffiliationMaterialsAndDoDefaultSetup()
        {
            MeshBlending[] co = Object.FindObjectsOfType<MeshBlending>();
            for (int i = 0; i < co.Length; i++)
            {
                co[i].SetupRenderersTerrainsMaterialsAndDoDefaultSetup();
            }
        }

        [MenuItem("Level Design/MeshBlending/UseSelectedTerrain")]
        public static void UseSelectedTerrain()
        {
            if (Selection.activeGameObject != null)
            {
                MeshBlending[] co = Object.FindObjectsOfType<MeshBlending>();
                for (int i = 0; i < co.Length; i++)
                {
                    co[i].UseSelectedTerrain(Selection.activeGameObject);
                    co[i].Setup(SetupType.WithoutCalculate);

                }
            }
        }

        [MenuItem("Level Design/MeshBlending/SetupForce")]
        public static void SetupObjectsForce()
        {
            MeshBlending[] co = Object.FindObjectsOfType<MeshBlending>();
            for (int i = 0; i < co.Length; i++)
            {
                co[i].Setup(SetupType.ForceBorders);
            }
        }

        [MenuItem("Level Design/Bake SkinnedMesh with LODs")]
        public static void Bake()
        {
            SkinnedMeshRenderer[] meshRenderer = Selection.activeGameObject.transform.GetComponentsInChildren<SkinnedMeshRenderer>();

            for (int i = 0; i < meshRenderer.Length; i++)
            {
                Mesh bakedMesh = new Mesh();
                meshRenderer[i].BakeMesh(bakedMesh);

                bakedMesh.RecalculateBounds();

                AssetDatabase.CreateAsset(bakedMesh, "Assets/" + meshRenderer[i].sharedMesh.name + ".asset");
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                AssetDatabase.SaveAssets();
            }
        }

        //[MenuItem("Level Design/MeshBlending/KillSourceMB")]
        public static void KillSourceMB()
        {
            string startName = Selection.activeGameObject.name;
            int index = startName.IndexOf('(');
            startName = startName.Substring(0, index - 1);
            Debug.Log("StartName:" + startName);

            var tempFolder = Path.Combine(Application.dataPath, startName);
            Debug.Log(tempFolder);
            if (!Directory.Exists(tempFolder))
                Directory.CreateDirectory(tempFolder);

            MeshBlending[] co = Object.FindObjectsOfType<MeshBlending>();
            List<GameObject> go = new List<GameObject>();
            for (int i = 0; i < co.Length; i++)
            {
                if (!go.Contains(co[i].gameObject))
                    go.Add(co[i].gameObject);
            }

            List<Preset> preset = new List<Preset>();

            for (int i = 0; i < go.Count; i++)
            {
                if (go[i].name.Length >= startName.Length)
                    if (go[i].name.Substring(0, startName.Length) == startName)
                    {
                        int selection = 0;
                        MeshBlending[] co1 = go[i].GetComponents<MeshBlending>();
                        for (int j = 0; j < co1.Length; j++)
                        {
                            if (co1[j].isSetupActive)
                                selection = j;
                        }

                        preset.Add(new Preset(co1[selection]));
                        AssetDatabase.CreateAsset(preset[preset.Count - 1], "Assets/" + startName + "/" + go[i].GetInstanceID() + ".preset");

                        Debug.Log(go[i].name);
                    }
            }

            MeshBlending[] mb = Selection.activeGameObject.GetComponents<MeshBlending>();

            int count = mb.Length;
            for (int i = 0; i < count; i++)
                GameObject.DestroyImmediate(mb[i]);

            PrefabUtility.ReplacePrefab(Selection.activeGameObject, PrefabUtility.GetCorrespondingObjectFromSource(Selection.activeGameObject), ReplacePrefabOptions.ConnectToPrefab);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();



            for (int i = 0; i < go.Count; i++)
            {
                for (int j = 0; j < preset.Count; j++)
                {
                    if (go[i].GetInstanceID().ToString().Equals(preset[j].name))
                    {
                        Debug.Log(i + " | " + preset[j].name);
                        MeshBlending mbThis = go[i].GetComponent<MeshBlending>();
                        if (mbThis == null)
                            mbThis = go[i].AddComponent<MeshBlending>();
                        preset[j].ApplyTo(mbThis);
                    }
                }
            }


        }


        [MenuItem("Level Design/Bonus/TerrainShadowsOn")]
        public static void SwitchOnTerrainShadowsTrue()
        {
            SwitchOnTerrainShadows(true);
        }

        [MenuItem("Level Design/Bonus/TerrainShadowsOff")]
        public static void SwitchOnTerrainShadowsFalse()
        {
            SwitchOnTerrainShadows(false);
        }
        public static void SwitchOnTerrainShadows(bool isTrue)
        {
            Terrain[] terrains = Object.FindObjectsOfType<Terrain>();
            for (int i = 0; i < terrains.Length; i++)
            {
                terrains[i].castShadows = isTrue;
            }
        }

        [MenuItem("Tools/UI/Add Gamma workflow to scene Images")]
        public static void AddUIGammaMaterialToSceneImages()
        {
            Material uiDefaultGamma = AssetDatabase.LoadAssetAtPath("Assets/UI/UIMaterial.mat", typeof(Material)) as Material;
            UnityEngine.UI.Image[] sceneImages = Object.FindObjectsOfType<UnityEngine.UI.Image>();
            if (sceneImages != null)
                for (int i = 0; i < sceneImages.Length; i++)
                {
                    if (sceneImages[i].material != null)
                    {
                        if (sceneImages[i].material.name == "Default UI Material")
                            sceneImages[i].material = uiDefaultGamma;
                    }

                }
        }

    }
}