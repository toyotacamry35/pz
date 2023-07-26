using System.Collections.Generic;
using Core.Environment.Logging.Extension;
using NLog;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using Logger = NLog.Logger;

// Simple example of stripping of a debug build configuration
class ShaderVariantsStripperDebug : IPreprocessShaders
{
    private static readonly Logger Logger = LogManager.GetLogger("BuildProcess");
    
    ShaderKeyword m_KeywordDebug;

    public ShaderVariantsStripperDebug()
    {
        m_KeywordDebug = new ShaderKeyword("DEBUG");
    }

    public int callbackOrder { get { return (int)ShaderVariantsStripperOrder.BuildConfiguration; } }

    public void OnProcessShader(
        Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> shaderVariants)
    {
        var inputShaderVariantCount = shaderVariants.Count;

        // For development build, don't strip debug variants
        for (var i = 0; i < shaderVariants.Count && !EditorUserBuildSettings.development; ++i)
        {
            if (!shaderVariants[i].shaderKeywordSet.IsEnabled(m_KeywordDebug)) continue;
            shaderVariants.RemoveAt(i);
            --i;
        }

        if (shaderVariants.Count == inputShaderVariantCount) return;
        
        var percentage = (float)shaderVariants.Count / (float)inputShaderVariantCount * 100f;
        Logger.IfInfo()?.Message(YText("DEBUG(" + snippet.shaderType.ToString() + ") = Kept / Total = " + shaderVariants.Count + " / " + inputShaderVariantCount + " = " + percentage + "% of the generated shader variants remain in the player data")).Write();
    }

    private string YText(string text) { return "<color=#ff0>" + text + "</color>"; }
}
