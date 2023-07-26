using System.Collections.Generic;
using System.Linq;
using Assets.Test.Src.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public class ShaderVariantStrippingForPlayer : IPreprocessShaders
{
    public int callbackOrder => (int)ShaderVariantsStripperOrder.Project;

    private ShaderVariantCollection _collection;

    private void LoadShaderVariant()
    {
        _collection = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>("Assets/Data/DataAssets/GraphicsSettings/IncludedShaderVariants_1082.shadervariants");
    }
    
    public bool KeepVariant(Shader shader, ShaderSnippetData snippet, ShaderCompilerData shaderVariant)
    {
        if(_collection == null)
            LoadShaderVariant();

        //var keywords = shaderVariant.shaderKeywordSet.GetShaderKeywords().Select(k => k.GetKeywordName()).ToArray();
        //var checkVariant = new ShaderVariantCollection.ShaderVariant {shader = shader, passType = snippet.passType, keywords = keywords};
        //_collection.Contains()
        //if (shader.name == "UI/Default" || 
        //    shader.name == "Sprites/Default")
        //return _collection.Contains(checkVariant);
        return true;

        // return !BuildPlayerCommon.IsLockExist();
    }

    public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> shaderVariants)
    {
        for (int i = 0; i < shaderVariants.Count; ++i)
        {
            var keepVariant = KeepVariant(shader, snippet, shaderVariants[i]);
            if (keepVariant) 
                continue;
            shaderVariants.RemoveAt(i);
            --i;
        }
    }
}