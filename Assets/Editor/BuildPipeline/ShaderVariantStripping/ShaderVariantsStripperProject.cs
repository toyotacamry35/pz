using System.Collections.Generic;
using Core.Environment.Logging.Extension;
using NLog;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using Logger = NLog.Logger;

// Simple example of stripping of a shader variants of 'ShaderVariantsStripping' shader.
class ShaderVariantsStripperProject : IPreprocessShaders
{
    private static readonly Logger Logger = LogManager.GetLogger("BuildProcess");

    ShaderKeyword m_KeywordColorOrange;
    ShaderKeyword m_KeywordColorViolet;
    ShaderKeyword m_KeywordColorGreen;

    ShaderKeyword m_KeywordOpAdd;
    ShaderKeyword m_KeywordOpMul;

    ShaderKeyword m_KeywordAddColorOrange;
    ShaderKeyword m_KeywordMulColorViolet;
    ShaderKeyword m_KeywordMulColorGreen;

    public ShaderVariantsStripperProject()
    {
        m_KeywordColorOrange = new ShaderKeyword("COLOR_ORANGE");
        m_KeywordColorViolet = new ShaderKeyword("COLOR_VIOLET");
        m_KeywordColorGreen = new ShaderKeyword("COLOR_GREEN");

        m_KeywordOpAdd = new ShaderKeyword("OP_ADD");
        m_KeywordOpMul = new ShaderKeyword("OP_MUL");

        m_KeywordAddColorOrange = new ShaderKeyword("ADD_COLOR_ORANGE");
        m_KeywordMulColorViolet = new ShaderKeyword("MUL_COLOR_VIOLET");
        m_KeywordMulColorGreen = new ShaderKeyword("MUL_COLOR_GREEN");
    }

    public int callbackOrder
    {
        get { return (int) ShaderVariantsStripperOrder.Project; }
    }

    public bool KeepVariant_ShaderVariantsStripping(ShaderSnippetData snippet, ShaderCompilerData shaderVariant)
    {
        bool addColor = shaderVariant.shaderKeywordSet.IsEnabled(m_KeywordColorOrange);
        if (addColor && shaderVariant.shaderKeywordSet.IsEnabled(m_KeywordOpAdd))
            return true;

        bool mulColor = shaderVariant.shaderKeywordSet.IsEnabled(m_KeywordColorGreen) || shaderVariant.shaderKeywordSet.IsEnabled(m_KeywordColorViolet);
        if (mulColor && shaderVariant.shaderKeywordSet.IsEnabled(m_KeywordOpMul) && snippet.shaderType == ShaderType.Fragment)
            return true;

        return false;
    }

    public bool KeepVariant_ShaderVariantsOptimized(ShaderSnippetData snippet, ShaderCompilerData shaderVariant)
    {
        if (shaderVariant.shaderKeywordSet.IsEnabled(m_KeywordAddColorOrange))
            return true;

        // We only need one vertex shader variant. We keep the one from ADD_COLOR_ORANGE.
        // This is because none of the keywords are used in the vertex shader stage.
        if (snippet.shaderType == ShaderType.Fragment)
        {
            if (shaderVariant.shaderKeywordSet.IsEnabled(m_KeywordMulColorViolet) ||
                shaderVariant.shaderKeywordSet.IsEnabled(m_KeywordMulColorGreen))
                return true;
        }

        return false;
    }

    public bool KeepVariant(Shader shader, ShaderSnippetData snippet, ShaderCompilerData shaderVariant)
    {
        if (shader.name == "ShaderVariantsStripping")
            return KeepVariant_ShaderVariantsStripping(snippet, shaderVariant);

        if (shader.name == "ShaderVariantsOptimized")
            return KeepVariant_ShaderVariantsOptimized(snippet, shaderVariant);

        // What do we do with the others shader variants? Here we choose to strip them all
        return true;
    }

    public void OnProcessShader(
        Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> shaderVariants)
    {
        var inputShaderVariantCount = shaderVariants.Count;

        for (var i = 0; i < shaderVariants.Count; ++i)
        {
            var keepVariant = KeepVariant(shader, snippet, shaderVariants[i]);
            if (keepVariant) continue;
            shaderVariants.RemoveAt(i);
            --i;
        }

        if (shaderVariants.Count == inputShaderVariantCount) return;
        
        var percentage = (float) shaderVariants.Count / inputShaderVariantCount * 100f;
        Logger.IfInfo()?.Message(BText("STRIPPING(" + snippet.shaderType.ToString() + ") = Kept / Total = " + shaderVariants.Count + " / " + inputShaderVariantCount + " = " + percentage + "% of the generated shader variants remain in the player data")).Write();
    }

    private string BText(string text)
    {
        return "<color=#44f>" + text + "</color>";
    }
}