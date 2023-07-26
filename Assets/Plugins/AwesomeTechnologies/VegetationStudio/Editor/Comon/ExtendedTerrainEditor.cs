using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Terrain))]
public class ExtendedTerrainEditor : DecoratorEditor
{
    private Terrain _terrain;
    private AwesomeTechnologies.VegetationSystem _vegetationSystem;
    private AwesomeTechnologies.TerrainSystem _terrainSystem;
    private Bounds _combindedBounds;

    public ExtendedTerrainEditor() : base("TerrainInspector")
    {

    }

    public virtual void Awake()
    {
        _terrain = (Terrain) target;

        AwesomeTechnologies.VegetationSystem[] vegetationSystems =
            FindObjectsOfType<AwesomeTechnologies.VegetationSystem>();
        for (int i = 0; i <= vegetationSystems.Length - 1; i++)
        {
            if (vegetationSystems[i].GetTerrain() == _terrain) _vegetationSystem = vegetationSystems[i];
        }

        AwesomeTechnologies.TerrainSystem[] terrainSystems = FindObjectsOfType<AwesomeTechnologies.TerrainSystem>();
        for (int i = 0; i <= terrainSystems.Length - 1; i++)
        {
            if (terrainSystems[i].GetTerrain() == _terrain) _terrainSystem = terrainSystems[i];
        }
    }

    public override void OnInspectorGUI()
    {
        if (_terrainSystem || _vegetationSystem)
        {
            int buttonIndex = GetActiveButton();
            if (buttonIndex == 3)
            {
                GUILayout.BeginVertical("box");
                EditorGUILayout.HelpBox(
                    "Vegetation will be updated automaticaly based on the spawn rules when drawing textures in the terrain.",
                    MessageType.Info);
                if (_terrainSystem)
                {
                    if (_terrainSystem.AutomaticApply)
                    {
                        EditorGUILayout.HelpBox(
                            "Splat map for terrain changes will be done automatic. Layers not selected for splat generation will kept",
                            MessageType.Info);
                    }
                }
                GUILayout.EndVertical();
            }

            if (buttonIndex == 0 || buttonIndex == 1 || buttonIndex == 2)
            {
                GUILayout.BeginVertical("box");
                if (_terrainSystem)
                {
                    if (_terrainSystem.AutomaticApply)
                    {
                        EditorGUILayout.HelpBox(
                            "Splat map for terrain changes will be done automatic. Layers not selected for splat generation will kept",
                            MessageType.Info);
                    }
                }
                GUILayout.EndVertical();
            }
        }
        base.OnInspectorGUI();
    }

    public override void OnSceneGUI()
    {
        bool LargeTerrain = true;

        Bounds editBounds = new Bounds();
        bool updateVegetation = false;

        _terrain = (Terrain) target;
        int buttonIndex = GetActiveButton();

        Event current = Event.current;
        int controlId = GUIUtility.GetControlID(EditorInstance.GetHashCode(), FocusType.Passive);
        var currentEventType = current.GetTypeForControl(controlId);

        if (buttonIndex <= 3)
        {
            switch (currentEventType)
            {
                case EventType.MouseDown:
                case EventType.MouseUp:
                case EventType.MouseDrag:
                    if (current.button == 0)
                    {
                        var size = EditorPrefs.GetInt("TerrainBrushSize");
                        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                        RaycastHit raycastHit;
                        if (_terrain.GetComponent<Collider>().Raycast(ray, out raycastHit, float.PositiveInfinity))
                        {
                            var pos = raycastHit.point;
                            float sizeScale = 5f;
                            editBounds = new Bounds(pos, new Vector3(size * sizeScale, size, size * sizeScale));
                            updateVegetation = true;
                        }
                    }
                    break;
            }

            if (currentEventType == EventType.MouseDown)
            {
                _combindedBounds = new Bounds(editBounds.center, editBounds.size);

            }
            else if (currentEventType == EventType.MouseDrag || currentEventType == EventType.MouseUp)
            {
                _combindedBounds.Encapsulate(editBounds);
            }
            //Handles.color = Color.red;
            //Handles.DrawWireCube(_combindedBounds.center, _combindedBounds.size);
        }

        base.OnSceneGUI();

        if (updateVegetation)
        {
            if (buttonIndex < 3)
            {
                if (LargeTerrain && currentEventType == EventType.MouseUp)
                {
                    _combindedBounds.Expand(4f);
                    if (_terrainSystem && _terrainSystem.AutomaticApply)
                    {
                        _terrainSystem.GenerateTerrainSplatMap(_combindedBounds, true, false);
                    }

                    if (_vegetationSystem)
                    {
                        _vegetationSystem.RefreshHeightMap(_combindedBounds, true, true);
                    }
                }
                else
                {
                    if (!LargeTerrain)
                    {

                        if (_terrainSystem && _terrainSystem.AutomaticApply)
                        {
                            _terrainSystem.GenerateTerrainSplatMap(editBounds, true);
                        }

                        if (_vegetationSystem)
                        {
                            _vegetationSystem.RefreshHeightMap(editBounds, true, true);
                        }
                    }
                }
            }
            else
            {
                editBounds.Expand(1f);
                if (_vegetationSystem)
                {
                    _vegetationSystem.RefreshHeightMap(editBounds, true, false);
                }
            }
        }

        //if (currentEventType == EventType.MouseUp)
        //{
        //    SaveBrushesToDisk();
        //}



    }

//    void SaveBrushesToDisk()
//    {
//        Texture2D[] brushArray = GetTextureArrayProperty("s_BrushTextures");
//        for (int i = 0; i <= brushArray.Length - 1; i++)
//        {
//            SaveTexture(brushArray[i], "D:\\Brushes\\Brush_" + i.ToString() + ".png");
//        }

//        Debug.Log(brushArray.Length); 
//    }

//    public static void SaveTexture(Texture2D tex, string name)
//    {
//#if UNITY_EDITOR
//        var bytes = tex.EncodeToPNG();
//        File.WriteAllBytes(name + ".png", bytes);
//#endif
//    }

    int GetActiveButton()
    {
        var property = decoratedEditorType.GetProperty("selectedTool",
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        return (int) property.GetValue(EditorInstance, EMPTY_ARRAY);
    }

    float GetFloatProperty(string parameterName)
    {
        var property = decoratedEditorType.GetProperty(parameterName,
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        return (float) property.GetValue(EditorInstance, EMPTY_ARRAY);
    }


    Texture2D[] GetTextureArrayProperty(string parameterName)
    {
        var property = decoratedEditorType.GetField(parameterName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        return (Texture2D[])property.GetValue(EditorInstance);
    }

}
