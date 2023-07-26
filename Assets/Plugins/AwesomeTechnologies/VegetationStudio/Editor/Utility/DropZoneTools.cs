using System;
using AwesomeTechnologies.MeshTerrains;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AwesomeTechnologies.Utility
{
    public enum DropZoneType
    {
        GrassPrefab,
        PlantPrefab,
        TreePrefab,
        ObjectPrefab,
        LargeObjectPrefab,
        GrassTexture,
        PlantTexture,
        MeshRenderer,
        Terrain
    }

    public class DropZoneTools
    {
        public static Type GetDropZoneSystemType(DropZoneType dropZoneType)
        {
            switch (dropZoneType)
            {
                case DropZoneType.GrassPrefab:
                    return typeof(GameObject);
                case DropZoneType.PlantPrefab:
                    return typeof(GameObject);
                case DropZoneType.TreePrefab:
                    return typeof(GameObject);
                case DropZoneType.ObjectPrefab:
                    return typeof(GameObject);
                case DropZoneType.LargeObjectPrefab:
                    return typeof(GameObject);
                case DropZoneType.GrassTexture:
                    return typeof(Texture2D);
                case DropZoneType.PlantTexture:
                    return typeof(Texture2D);
                case DropZoneType.MeshRenderer:
                    return typeof(MeshRenderer);
                case DropZoneType.Terrain:
                    return typeof(Terrain);
                default:
                    return typeof(GameObject);
            }
        }

        public static void DrawVegetationItemDropZone(DropZoneType dropZoneType, VegetationSystem vegetationSystem, ref Boolean addedItem)
        {
            Event evt = Event.current;

            Type selectedType = GetDropZoneSystemType(dropZoneType);
            Texture2D iconTexture = GetDropZoneIconTexture(dropZoneType);

            Rect dropArea = GUILayoutUtility.GetRect(iconTexture.width, iconTexture.height, GUILayout.ExpandWidth(false));
            GUILayoutUtility.GetRect(5, iconTexture.height, GUILayout.ExpandWidth(false));
            EditorGUI.DrawPreviewTexture(dropArea, iconTexture);

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:

                    if (!dropArea.Contains(evt.mousePosition))
                    {
                        return;
                    }

                    bool hasType = HasDropType(DragAndDrop.objectReferences, selectedType);
                    if (!hasType) return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (Object draggedObject in DragAndDrop.objectReferences)
                        {
                            if (draggedObject.GetType() == selectedType)
                            {
                                switch (dropZoneType)
                                {
                                    case DropZoneType.GrassTexture:
                                        {
                                            GameObject defaultTexturePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/GrassSystem/_Resources/DefaultGrassPatch.prefab");
                                            vegetationSystem.currentVegetationPackage.AddVegetationItem(draggedObject as Texture2D, defaultTexturePrefab, VegetationType.Grass, true);
                                        }
                                        break;
                                    case DropZoneType.PlantTexture:
                                        {
                                            GameObject defaultTexturePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/GrassSystem/_Resources/DefaultGrassPatch.prefab");
                                            vegetationSystem.currentVegetationPackage.AddVegetationItem(draggedObject as Texture2D, defaultTexturePrefab, VegetationType.Plant, true);
                                        }
                                        break;
                                    case DropZoneType.GrassPrefab:
                                        vegetationSystem.currentVegetationPackage.AddVegetationItem(draggedObject as GameObject, VegetationType.Grass, true);
                                        break;
                                    case DropZoneType.PlantPrefab:
                                        vegetationSystem.currentVegetationPackage.AddVegetationItem(draggedObject as GameObject, VegetationType.Plant, true);
                                        break;
                                    case DropZoneType.TreePrefab:
                                        vegetationSystem.currentVegetationPackage.AddVegetationItem(draggedObject as GameObject, VegetationType.Tree, true);
                                        break;
                                    case DropZoneType.ObjectPrefab:
                                        vegetationSystem.currentVegetationPackage.AddVegetationItem(draggedObject as GameObject, VegetationType.Objects, true);
                                        break;
                                    case DropZoneType.LargeObjectPrefab:
                                        vegetationSystem.currentVegetationPackage.AddVegetationItem(draggedObject as GameObject, VegetationType.LargeObjects, true);
                                        break;
                                }

                                addedItem = true;
                            }
                        }
                    }
                    break;
            }

        }


        public static void DrawVegetationItemDropZone(DropZoneType dropZoneType, VegetationPackage package, ref Boolean addedItem)
        {
            Event evt = Event.current;

            Type selectedType = GetDropZoneSystemType(dropZoneType);
            Texture2D iconTexture = GetDropZoneIconTexture(dropZoneType);

            Rect dropArea = GUILayoutUtility.GetRect(iconTexture.width, iconTexture.height, GUILayout.ExpandWidth(false));
            GUILayoutUtility.GetRect(5, iconTexture.height, GUILayout.ExpandWidth(false));
            EditorGUI.DrawPreviewTexture(dropArea, iconTexture);

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:

                    if (!dropArea.Contains(evt.mousePosition))
                    {
                        return;
                    }

                    bool hasType = HasDropType(DragAndDrop.objectReferences, selectedType);
                    if (!hasType) return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (Object draggedObject in DragAndDrop.objectReferences)
                        {
                            if (draggedObject.GetType() == selectedType)
                            {
                                switch (dropZoneType)
                                {
                                    /*
                                    case DropZoneType.GrassTexture:
                                        package.AddVegetationItem(draggedObject as Texture2D, VegetationType.Grass, true);
                                        break;
                                    case DropZoneType.PlantTexture:
                                        package.AddVegetationItem(draggedObject as Texture2D, VegetationType.Plant, true);
                                        break;
                                    */
                                    case DropZoneType.GrassPrefab:
                                        package.AddVegetationItem(draggedObject as GameObject, VegetationType.Grass, true);
                                        break;
                                    case DropZoneType.PlantPrefab:
                                        package.AddVegetationItem(draggedObject as GameObject, VegetationType.Plant, true);
                                        break;
                                    case DropZoneType.TreePrefab:
                                        package.AddVegetationItem(draggedObject as GameObject, VegetationType.Tree, true);
                                        break;
                                    case DropZoneType.ObjectPrefab:
                                        package.AddVegetationItem(draggedObject as GameObject, VegetationType.Objects, true);
                                        break;
                                        /*
                                    case DropZoneType.LargeObjectPrefab:
                                        package.AddVegetationItem(draggedObject as GameObject, VegetationType.LargeObjects, true);
                                        break;
                                        */
                                }

                                addedItem = true;
                            }
                        }
                    }
                    break;
            }

        }
        public static void DrawMeshTerrainDropZone(DropZoneType dropZoneType, MeshTerrain meshTerrain, ref Boolean addedItem)
        {
            Event evt = Event.current;
            Texture2D iconTexture = GetDropZoneIconTexture(dropZoneType);

            Rect dropArea = GUILayoutUtility.GetRect(iconTexture.width, iconTexture.height, GUILayout.ExpandWidth(false));
            GUILayoutUtility.GetRect(5, iconTexture.height, GUILayout.ExpandWidth(false));
            EditorGUI.DrawPreviewTexture(dropArea, iconTexture);

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:

                    if (!dropArea.Contains(evt.mousePosition))
                    {
                        return;
                    }

                    bool hasType = HasDropComponentType(DragAndDrop.objectReferences, dropZoneType);
                    if (!hasType) return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (Object draggedObject in DragAndDrop.objectReferences)
                        {
                            GameObject droppedgo;
                            switch (dropZoneType)
                            {
                                case DropZoneType.Terrain:
                                    droppedgo = draggedObject as GameObject;
                                    if (!droppedgo) break;
                                    addedItem = true;
                                    meshTerrain.AddTerrain(droppedgo,MeshTerrainSourceType.MeshTerrainSource1);                                    
                                    break;
                                case DropZoneType.MeshRenderer:
                                    droppedgo = draggedObject as GameObject;
                                    if (!droppedgo) break;
                                    addedItem = true;
                                    meshTerrain.AddMeshRenderer(droppedgo,MeshTerrainSourceType.MeshTerrainSource1);
                                    break;
                            }                            
                        }
                    }
                    break;
            }
        }

        static Texture2D GetDropZoneIconTexture(DropZoneType dropZoneType)
        {
            if (EditorGUIUtility.isProSkin)
            {
                switch (dropZoneType)
                {                        
                    case DropZoneType.Terrain:
                        return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/DropZoneIcons/DarkTerrainDropZoneNoBorder.png");
                    case DropZoneType.MeshRenderer:
                        return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/DropZoneIcons/DarkMeshDropZoneNoBorder.png");
                    case DropZoneType.GrassPrefab:
                        return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/DropZoneIcons/DarkGrassPrefabNoBorder.psd");
                    case DropZoneType.PlantPrefab:
                        return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/DropZoneIcons/DarkPlantPrefabNoBorder.psd");
                    case DropZoneType.TreePrefab:
                        return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/DropZoneIcons/DarkTreePrefabNoBorder.psd");
                    case DropZoneType.ObjectPrefab:
                        return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/DropZoneIcons/DarkObjectPrefabNoBorder.psd");
                    case DropZoneType.LargeObjectPrefab:
                        return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/DropZoneIcons/DarkLargeObjectPrefabNoBorder.psd");
                    case DropZoneType.GrassTexture:
                        return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/DropZoneIcons/DarkGrassTextureNoBorder.psd");
                    case DropZoneType.PlantTexture:
                        return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/DropZoneIcons/DarkPlantTextureNoBorder.psd");
                    default:
                        return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/DropZoneIcons/DarkPlantPrefabNoBorder.psd");
                }
            }
            else
            {
                switch (dropZoneType)
                {
                    case DropZoneType.Terrain:
                        return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/DropZoneIcons/DarkTerrainDropZoneNoBorder.png");
                    case DropZoneType.MeshRenderer:
                        return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/DropZoneIcons/DarkMeshDropZoneNoBorder.png");
                    case DropZoneType.GrassPrefab:
                        return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/DropZoneIcons/LightGrassPrefabNoBorder.psd");
                    case DropZoneType.PlantPrefab:
                        return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/DropZoneIcons/LightPlantPrefabNoBorder.psd");
                    case DropZoneType.TreePrefab:
                        return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/DropZoneIcons/LightTreePrefabNoBorder.psd");
                    case DropZoneType.ObjectPrefab:
                        return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/DropZoneIcons/LightObjectPrefabNoBorder.psd");
                    case DropZoneType.LargeObjectPrefab:
                        return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/DropZoneIcons/LightLargeObjectPrefabNoBorder.psd");
                    case DropZoneType.GrassTexture:
                        return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/DropZoneIcons/LightGrassTextureNoBorder.psd");
                    case DropZoneType.PlantTexture:
                        return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/DropZoneIcons/LightPlantTextureNoBorder.psd");
                    default:
                        return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/DropZoneIcons/LightPlantPrefabNoBorder.psd");
                }
            }
        }

        private static bool HasDropType(Object[] dragObjects, System.Type type)
        {
            foreach (Object draggedObject in dragObjects)
            {
                if (draggedObject.GetType() != type) continue;
                return true;
            }

            return false;
        }

        private static bool HasDropComponentType(Object[] dragObjects, DropZoneType dropZoneType)
        {
            foreach (Object draggedObject in dragObjects)
            {
                GameObject draggedGo = draggedObject as GameObject;
                if (!draggedGo) continue;

                if (dropZoneType == DropZoneType.MeshRenderer)
                {
                    if (draggedGo.GetComponentInChildren<MeshRenderer>() != null) return true;
                }

                if (dropZoneType == DropZoneType.Terrain)
                {
                    if (draggedGo.GetComponentInChildren<Terrain>() != null) return true;
                }
            }
            return false;
        }
    }
}

