//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth, slipster216@gmail.com
//////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace JBooth.MicroSplat
{
   class TextureArrayPreProcessor : AssetPostprocessor 
   {
      static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) 
      {
         var cfgs = AssetDatabase.FindAssets("t:TextureArrayConfig");
         for (int i = 0; i < cfgs.Length; ++i)
         {
            var asset = AssetDatabase.GUIDToAssetPath(cfgs[i]);
            var cfg = AssetDatabase.LoadAssetAtPath<TextureArrayConfig>(asset);
            if (cfg != null)
            {
               int hash = cfg.GetNewHash();
               if (hash != cfg.hash)
               {
                  cfg.hash = hash;
                  TextureArrayConfigEditor.CompileConfig(cfg);
                  EditorUtility.SetDirty(cfg);
               }
            }
         }
      }
   }
}
