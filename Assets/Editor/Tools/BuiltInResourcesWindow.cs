//From http://wiki.unity3d.com/index.php/Show_Built_In_Resources
using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Assets.Src.Lib.ProfileTools;

public class BuiltInResourcesWindow : EditorWindow
{
    [MenuItem("Window/Built-in styles and icons")]
    public static void ShowWindow()
    {
        BuiltInResourcesWindow w = GetWindow<BuiltInResourcesWindow>();
        w.Show();
    }

    private struct Drawing
    {
        public Rect Rect;
        public GUIStyle GuiStyle;
        public float Width;
        public Texture2D Texture2D;
    }

    private List<Drawing> _drawings;

    private List<Texture2D> _textures2D;
    private float _scrollPos;
    private float _maxY;
    private Rect _oldPosition;

    private bool _showingStyles = true;
    private GUIContent toggleContent = new GUIContent("Toggle");
    private GUIContent labelContent = new GUIContent("Label");


    private string _search = "";

    void OnIMGUI()
    {
        if (!Mathf.Approximately(position.width, _oldPosition.width) && Event.current.type == EventType.Layout)
        {
            _drawings = null;
            _oldPosition = position;
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Toggle(_showingStyles, "Styles", EditorStyles.toolbarButton) != _showingStyles)
        {
            _showingStyles = !_showingStyles;
            _drawings = null;
        }

        if (GUILayout.Toggle(!_showingStyles, "Icons", EditorStyles.toolbarButton) == _showingStyles)
        {
            _showingStyles = !_showingStyles;
            _drawings = null;
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Filter", GUILayout.MaxWidth(50));
        var lastSearch = _search;
        _search = EditorGUILayout.TextField(_search, GUILayout.MaxWidth(400));
        EditorGUILayout.Separator();
        GUILayout.EndHorizontal();
        if (lastSearch != _search)
        {
            _drawings = null;
        }

        float top = 36;

        if (_drawings == null)
        {
            lastSearch = _search.ToLower();

            _drawings = new List<Drawing>();

            float x = 5.0f;
            float y = 5.0f;

            if (_showingStyles)
            {
                foreach (GUIStyle guiStyle in GUI.skin.customStyles)
                {
                    if (lastSearch != "" && !guiStyle.name.ToLower().Contains(lastSearch))
                        continue;

                    //Logs.Log("guiStyle.name.ToLower()='{0}'", guiStyle.name.ToLower());
                    Drawing draw = new Drawing();

                    float width = 16 + Mathf.Max(
                        100.0f,
                        GUI.skin.button.CalcSize(new GUIContent(guiStyle.name)).x,
                        guiStyle.CalcSize(toggleContent).x + guiStyle.CalcSize(labelContent).x);

                    float height = 60.0f;

                    if (x + width > position.width - 32 && x > 5.0f)
                    {
                        x = 5.0f;
                        y += height + 10.0f;
                    }

                    draw.Rect = new Rect(x, y, width, height);

                    width -= 8.0f;

                    draw.GuiStyle = guiStyle;
                    draw.Width = width;
                    x += width + 18.0f;

                    _drawings.Add(draw);
                }
            }
            else
            {
                if (_textures2D == null)
                {
                    _textures2D = new List<Texture2D>(Profile.FindObjectsOfTypeAll<Texture2D>());
                    _textures2D.Sort((pA, pB) => string.Compare(pA.name, pB.name, StringComparison.OrdinalIgnoreCase));
                }

                float rowHeight = 0.0f;

                foreach (Texture2D texture2D in _textures2D)
                {
                    if (texture2D == null || texture2D.name == "")
                        continue;

                    if (lastSearch != "" && !texture2D.name.ToLower().Contains(lastSearch))
                        continue;

                    Drawing draw = new Drawing();

                    float width = Mathf.Max(
                        GUI.skin.button.CalcSize(new GUIContent(texture2D.name)).x,
                        texture2D.width
                    ) + 8.0f;

                    float height = texture2D.height + GUI.skin.button.CalcSize(new GUIContent(texture2D.name)).y + 8.0f;

                    if (x + width > position.width - 32.0f)
                    {
                        x = 5.0f;
                        y += rowHeight + 8.0f;
                        rowHeight = 0.0f;
                    }

                    draw.Rect = new Rect(x, y, width, height);

                    rowHeight = Mathf.Max(rowHeight, height);

                    width -= 8.0f;

                    draw.Texture2D = texture2D;
                    draw.Width = width;
                    x += width + 8.0f;
                    _drawings.Add(draw);
                }
            }

            _maxY = y;
        }

        Rect r = position;
        r.y = top;
        r.height -= r.y;
        r.x = r.width - 16;
        r.width = 16;

        float areaHeight = position.height - top;
        _scrollPos = GUI.VerticalScrollbar(r, _scrollPos, areaHeight, 0.0f, _maxY);

        Rect area = new Rect(0, top, position.width - 16.0f, areaHeight);
        GUILayout.BeginArea(area);

        foreach (Drawing draw in _drawings)
        {
            Rect newRect = draw.Rect;
            newRect.y -= _scrollPos;

            if (newRect.y + newRect.height > 0 && newRect.y < areaHeight)
            {
                GUILayout.BeginArea(newRect, GUI.skin.textField);
                if (_showingStyles)
                {
                    if (GUILayout.Button(draw.GuiStyle.name, GUILayout.Width(draw.Width)))
                        CopyText(draw.GuiStyle.name);

                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    GUILayout.Toggle(true, toggleContent, draw.GuiStyle, GUILayout.Width(draw.Width / 2));
                    GUILayout.Label(labelContent, draw.GuiStyle, GUILayout.Width(draw.Width / 2));
                    GUILayout.EndHorizontal();
                    GUILayout.TextField("text", draw.GuiStyle, GUILayout.Width(draw.Width / 2));
                    GUILayout.EndVertical();
                }
                else
                {
                    if (GUILayout.Button(draw.Texture2D.name, GUILayout.Width(draw.Width)))
                        CopyText(draw.Texture2D.name);

                    Rect textureRect = GUILayoutUtility.GetRect(
                        draw.Texture2D.width,
                        draw.Texture2D.width,
                        draw.Texture2D.height,
                        draw.Texture2D.height,
                        GUILayout.ExpandHeight(false),
                        GUILayout.ExpandWidth(false));
                    EditorGUI.DrawTextureTransparent(textureRect, draw.Texture2D);
                }
                GUILayout.EndArea();
            }
        }

        GUILayout.EndArea();
    }

    void CopyText(string pText)
    {
        TextEditor editor = new TextEditor { text = pText };
        editor.SelectAll();
        editor.Copy();
    }
}