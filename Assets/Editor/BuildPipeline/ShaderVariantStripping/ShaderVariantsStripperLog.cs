using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

class ShaderVariantsStripperLog : IPreprocessShaders
{
    static bool enableLogOnly = false;

    [MenuItem("Shader Variants Stripping/Enable Log Only")]
    static void EnableLogOnly()
    {
        enableLogOnly = true;
    }

    [MenuItem("Shader Variants Stripping/Disable Log Only")]
    static void DisableLogOnly()
    {
        enableLogOnly = false;
    }

    public ShaderVariantsStripperLog()
    {
    }

    public int callbackOrder
    {
        get { return (int) ShaderVariantsStripperOrder.Log; }
    }

    public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
    {
#if SBP_PROFILER_ENABLE
        LogVariantProcess(shader, snippet, data);
#endif
    }

    private void LogVariantProcess(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
    {
        string prefix = "VARIANT: " + shader.name + " (";
        if (snippet.passName.Length > 0)
            prefix += snippet.passName + ", ";

        prefix += snippet.shaderType.ToString() + ") ";

        for (int i = 0; i < data.Count; ++i)
        {
            string log = prefix;

            ShaderKeyword[] keywords = data[i].shaderKeywordSet.GetShaderKeywords();
            for (int labelIndex = 0; labelIndex < keywords.Count(); ++labelIndex)
                log += keywords[labelIndex].GetKeywordName() + " ";

            Debug.Log(GText(log));
        }

        if (enableLogOnly)
            data.Clear();
    }

    private string GText(string text)
    {
        return "<color=#0f0>" + text + "</color>";
    }

    private string YText(string text)
    {
        return "<color=#ff0>" + text + "</color>";
    }

    private string RText(string text)
    {
        return "<color=#f80>" + text + "</color>";
    }
}