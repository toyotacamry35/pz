using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using Core.Reflection;

namespace AwesomeTechnologies
{
    [System.Serializable]
    public class DocumentationEditorWindow : EditorWindow
    {
        //static string Url = "https://awesometechweb.azurewebsites.net/awesome-vegetation-system-component/";
        static string Url = "http://www.awesometech.no/awesome-vegetation-system-component/";
        public static string HelpTopic;
        public static ScriptableObject WebView;
        public static Type WebViewType;

        //[MenuItem("Window/Awesome Technologies/Documentation")]
        static void Init()
        {
            //DestroyWebView();
            var window = DocumentationEditorWindow.GetWindow<DocumentationEditorWindow>();
            window.Show();

            //Texture2D _icon = Profile.Load<Texture2D>("InfoIcon"));
            //Texture icon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Sprites/Gear.png");
            GUIContent titleContent = new GUIContent("Documentation");
            window.titleContent = titleContent;

            OpenWebView(window);

        }

        public static void ShowDocument(string HelpTopic)
        {
            DocumentationEditorWindow.HelpTopic = HelpTopic;
            Init();
        }

        static void OpenWebView(DocumentationEditorWindow window)
        {
            var thisWindowGuiView = typeof(EditorWindow).GetField("m_Parent", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(window);
            if (WebView == null)
            {
                WebViewType = GetTypeFromAllAssemblies("WebView");
                WebView = ScriptableObject.CreateInstance(WebViewType);
                Rect webViewRect = new Rect(0, 23, 1024, window.position.height);
                WebViewType.GetMethod("InitWebView").Invoke(WebView, new object[] { thisWindowGuiView, (int)webViewRect.x, (int)webViewRect.y, (int)webViewRect.width, (int)webViewRect.height, true });
                //webViewType.GetProperty("hideFlags").SetValue(webView, HideFlags.DontSave, new object[] { });

            }
            WebViewType.GetMethod("LoadURL").Invoke(WebView, new object[] { Url });
        }


        static void DestroyWebView()
        {
            if (WebView)
            {
                WebViewType.GetMethod("SetHostView").Invoke(WebView, new object[] { null });
                WebViewType.GetMethod("OnDestroy").Invoke(WebView, new object[] { });
                UnityEngine.Object.DestroyImmediate(WebView);
                WebView = null;
            }

        }



        public void OnFocus()
        {
            WebView_SetFocus(true);
        }

        public void OnEnable()
        {
            Init();
        }

        public void OnLostFocus()
        {
            WebView_SetFocus(false);
        }

        void OnGUI()
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (WebView)
                {
                    SetSizeAndPosition(0, 23, (int)this.position.width, (int)this.position.height);
                }
            }
        }

        public void Refresh()
        {
            WebView_Hide();
            WebView_Show();
        }

        void SetSizeAndPosition(int x, int y, int width, int height)
        {
            if (WebView)
            {
                WebViewType.GetMethod("SetSizeAndPosition").Invoke(WebView, new object[] { x, y, width, height });
            }
        }

        public void OnDestroy()
        {
            WebView_DestroyWebView();
        }



        public void OnDisable()
        {
            WebView_DestroyWebView();
        }

        public void OnBecameInvisible()
        {
            WebView_SetHostView(null);
            WebView_Hide();
        }

        public void OnBecameVisible()
        {
            WebView_SetHostView(this);
            WebView_Show();
        }

        void WebView_SetFocus(bool value)
        {
            if (WebView)
            {
                WebViewType.GetMethod("SetFocus").Invoke(WebView, new object[] { value });
            }
        }

        void WebView_SetHostView(EditorWindow window)
        {
            if (window == null)
            {
                if (WebViewType != null && WebView != null)
                {
                    WebViewType.GetMethod("SetHostView").Invoke(WebView, new object[] { null });
                }
            }
            else
            {
                var thisWindowGuiView = typeof(EditorWindow).GetField("m_Parent", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);
                if (WebView)
                {
                    WebViewType.GetMethod("SetHostView").Invoke(WebView, new object[] { thisWindowGuiView });
                }
            }
        }

        void WebView_DestroyWebView()
        {
            WebView_SetHostView(null);

            if (WebView)
            {
                WebViewType.GetMethod("OnDestroy").Invoke(WebView, new object[] { });
            }
            UnityEngine.Object.DestroyImmediate(WebView);
            WebView = null;
        }

        void WebView_Show()
        {
            if (WebView)
            {
                WebViewType.GetMethod("Show").Invoke(WebView, new object[] { });
            }
        }

        void WebView_Hide()
        {
            if (WebView)
            {
                WebViewType.GetMethod("Hide").Invoke(WebView, new object[] { });
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
}
