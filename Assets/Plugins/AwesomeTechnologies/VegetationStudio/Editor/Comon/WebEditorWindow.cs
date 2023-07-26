using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using Core.Reflection;

public class TerrainBuilderWebInterface: ScriptableObject
{
    public void SendMessage(string message, string WebViewV8CallbackCSharp)
    {
        //Debug.Log(message);
    }
}

public class WebEditorWindow : EditorWindow
{
    //static string Url = "http://google.com";
    static TerrainBuilderWebInterface _terrainBuilderWebInterface;

    static ScriptableObject webView;
    static Type webViewType;

    //[MenuItem("Window/Awesome Technologies/Terrain builder")]
    static void Init()
    {
      
        var window = WebEditorWindow.GetWindow<WebEditorWindow>();
        window.Show();

        //Texture icon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Sprites/Gear.png");
        GUIContent titleContent = new GUIContent("Terrain Builder");
        window.titleContent = titleContent;
        OpenWebView(window);
    }

    static void OpenWebView(WebEditorWindow window)
    {
        var thisWindowGuiView = typeof(EditorWindow).GetField("m_Parent", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(window);

        webViewType = GetTypeFromAllAssemblies("WebView");
        webView = ScriptableObject.CreateInstance(webViewType);

        Rect webViewRect = new Rect(0, 223, 1024, window.position.height);
        webViewType.GetMethod("InitWebView").Invoke(webView, new object[] { thisWindowGuiView, (int)webViewRect.x, (int)webViewRect.y, (int)webViewRect.width, (int)webViewRect.height, true });

        string filename = "G:\\Development\\WebView\\Test.html";
        webViewType.GetMethod("LoadFile").Invoke(webView, new object[] { filename });
        //webViewType.GetMethod("LoadURL").Invoke(webView, new object[] { Url });

        _terrainBuilderWebInterface = ScriptableObject.CreateInstance<TerrainBuilderWebInterface>();
        webViewType.GetMethod("DefineScriptObject").Invoke(webView, new object[] { "window.unityScriptObject", _terrainBuilderWebInterface });
        webViewType.GetMethod("AllowRightClickMenu").Invoke(webView, new object[] { true });
    }

    void OnGUI()
    {
        if (GUILayout.Button("Send javascript to browser"))
        {
            webViewType.GetMethod("ExecuteJavascript").Invoke(webView, new object[] { "alert('This is a javascript alert from the unity editor')" });
        }
    }

    public static Type GetTypeFromAllAssemblies(string typeName)
    {
        Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssembliesSafe();
        foreach (Assembly assembly in assemblies)
        {
            Type[] types = assembly.GetTypesSafe();
            foreach (Type type in types)
            {
                if (type.Name.Equals(typeName, StringComparison.CurrentCultureIgnoreCase) || type.Name.Contains('+' + typeName)) //+ check for inline classes
                    return type;
            }
        }
        return null;
    }
}