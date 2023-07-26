using System.Collections.Generic;
using System.IO;
using Assets.ColonyShared.SharedCode.Aspects.Craft;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourceSystem;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Wizardry;
using UnityEditor;
using UnityEngine;

namespace Assets.Src.ResourceSystem.Editor
{
    [InitializeOnLoad]
    class ResourceSystemIcons : AssetPostprocessor
    {
        private const string IconsPath = @"Assets/Editor/ResourceSystem/Icons/";
        private const string MenuItem = "Assets/Show JDB Icons";
        private const string JdbExtension = ".jdb";
        private static readonly JdbIconDef[] JdbIcons =
        {
            new JdbIconDef {Type = typeof(SpellDef).FullName, Icon = "spell_file.png"},
            new JdbIconDef {Type = typeof(BaseItemResource).FullName, Icon = "item_file.png"},
            new JdbIconDef {Type = typeof(CraftRecipeDef).FullName, Icon = "blueprint_file.png"},
            new JdbIconDef {Type = typeof(StatResource).FullName, Icon = "stat_file.png"},
            new JdbIconDef {Type = null, Icon = "bad_file.png"},
            new JdbIconDef {Type = "*", Icon = "jdb_file.png"},
        };

        private static readonly Dictionary<string,Texture2D> GuidToIcon = new Dictionary<string, Texture2D>();

        private static bool Enabled
        {
            get { return EditorPrefs.GetBool("ResourceSystemIcons.Enabled", true); }
            set { EditorPrefs.SetBool("ResourceSystemIcons.Enabled", value); }
        }
        
        public static Texture2D GetAssetIconByPath(string assetPath)
        {
            return GetAssetIconByGuid(AssetDatabase.AssetPathToGUID(assetPath));
        }

        public static Texture2D GetAssetIconByGuid(string guid)
        {
            Texture2D texture;
            if (!GuidToIcon.TryGetValue(guid, out texture))
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (assetPath.EndsWith(JdbExtension))
                {
                    var meta = AssetDatabase.LoadAssetAtPath<JdbMetadata>(assetPath);
                    var assetType = meta ? meta.Type : null; 
                    foreach (var def in JdbIcons)
                        if (def.Type == "*" || string.IsNullOrEmpty(assetType) && string.IsNullOrEmpty(def.Type) || assetType == def.Type)
                        {
                            GuidToIcon.Add(guid, texture = def.Texture);
                            return texture;
                        }
                }
            }
            return texture;
        }
        
        static ResourceSystemIcons()
        {
            foreach(var def in JdbIcons)
                def.Texture = AssetDatabase.LoadAssetAtPath(Path.Combine(IconsPath, def.Icon), typeof(Texture2D)) as Texture2D;
            if(Enabled)
                Enable();
        }

        private static void Enable()
        {
            //   EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
            //   EditorApplication.RepaintHierarchyWindow();
            EditorApplication.projectWindowItemOnGUI  += ProjectItemCB;
            EditorApplication.RepaintProjectWindow();
        }

        private static void Disable()
        {
            EditorApplication.projectWindowItemOnGUI  -= ProjectItemCB;
            EditorApplication.RepaintProjectWindow();
        }
        
   //     private static void HierarchyItemCB(int instanceID, Rect selectionRect)
   //     {
   //     }

        private static void ProjectItemCB(string guid, Rect selectionRect)
        {
            Texture2D texture = GetAssetIconByGuid(guid);
            if (texture != null)
            {
                int num = 0;
                DrawIcon(selectionRect, ref num, texture);
            }
        }
        
        private static void DrawIcon(Rect selectionRect, ref int num, Texture2D icon)
        {
            if (icon != null)
            {
                Rect r;
                    
                if(selectionRect.width > selectionRect.height)    
                    r = new Rect(selectionRect.xMin - (selectionRect.height + 2 ) * num, selectionRect.yMin, selectionRect.height, selectionRect.height);
                else
                    r = new Rect(selectionRect.xMin, selectionRect.yMin, selectionRect.width, selectionRect.width);
                //GUI.Box(r, new GUIContent());
                GUI.DrawTexture(r, icon);
                num++;
            }
        }
        
        
        [MenuItem(MenuItem, false, 5)]
        private static void MenuItemHandler()
        {
            Enabled = !Enabled;
            if(Enabled)
                Enable();
            else
                Disable();
        }
        
        [MenuItem(MenuItem, true)]
        private  static bool MenuItemCheckbox()
        {
            Menu.SetChecked(MenuItem, Enabled);
            return true;
        }
        
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            bool any = false;
            foreach (var assetPath in importedAssets)
                any |= UnregisterFile(assetPath);
            foreach (var assetPath in deletedAssets)
                any |= UnregisterFile(assetPath);
            if(any)
                EditorApplication.RepaintProjectWindow();
        }

        static bool UnregisterFile(string assetPath)
        {
            if (assetPath.EndsWith(JdbExtension))
            {
                var guid = AssetDatabase.AssetPathToGUID(assetPath);
                return GuidToIcon.Remove(guid);
            }
            return false;
        }

        class JdbIconDef
        {
            public string Type;
            public string Icon;
            public Texture2D Texture;
        }
    }
}