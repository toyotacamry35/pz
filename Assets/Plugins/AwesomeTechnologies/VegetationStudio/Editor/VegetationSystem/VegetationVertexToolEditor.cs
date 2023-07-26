using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AwesomeTechnologies.Utility;

namespace AwesomeTechnologies.VegetationStudio
{
    public enum PhaseBranchType
    {
        Default = 0,
        Discrete = 1,
    }

    public enum VertexButton
    {
        RGB = 0,
        R = 1,
        G = 2,
        B = 3,
        A = 4,
        UV = 5,
        UV2 = 6,
        UV3 = 7,
        UV4 = 8

    }
    [CustomEditor(typeof(VegetationVertexTool))]
    public class VegetationVertexToolEditor : Editor
    {
        private VegetationVertexTool current;
        private int windowDefaultSize = 512; // 512 + (sideSpace*2)
        private Mesh m = null;
        private int[] tris;
        private Vector2[] uvs;
        private int selectedUV = 0;
        private bool canDrawView;
        private bool mousePositionInsidePreview;
        private float xPanShift;
        private float yPanShift;
        private Rect uvPreviewRect;
        private float scale = 1;
        private Texture2D fillTextureGray;
        private Texture2D fillTextureDark;
        private Material lineMaterial;
        private string[] selectedUVStrings = new string[2];
        private int selectionPoint = -1;
        private bool _painting;
        private SceneMeshRaycaster _sceneMeshRaycaster;
        private bool isAltPressed = false;
        private PhaseBranchType phaseBranchType = PhaseBranchType.Default;
        private int brushPhase = 122;

        public override void OnInspectorGUI()
        {
            current = (VegetationVertexTool)target;
            //base.OnInspectorGUI();



            GUI.color = new Color(0.8f, 0.8f, 0.8f);
            EditorGUI.BeginChangeCheck();

            GUILayout.BeginHorizontal();


            if (current.selected == (int)VertexButton.RGB)
                GUI.color = new Color(0.34f, 0.34f, 0.34f);
            if (GUILayout.Button("RGB"))
                current.selected = 0;
            GUI.color = Color.white;

            if (current.selected == (int)VertexButton.R)
                GUI.color = new Color(0.34f, 0.34f, 0.34f);
            if (GUILayout.Button("R", EditorStyles.miniButtonLeft))
                current.selected = 1;
            GUI.color = Color.white;

            if (current.selected == (int)VertexButton.G)
                GUI.color = new Color(0.34f, 0.34f, 0.34f);
            if (GUILayout.Button("G", EditorStyles.miniButtonMid))
                current.selected = 2;
            GUI.color = Color.white;

            if (current.selected == (int)VertexButton.B)
                GUI.color = new Color(0.34f, 0.34f, 0.34f);
            if (GUILayout.Button("B", EditorStyles.miniButtonMid))
                current.selected = 3;
            GUI.color = Color.white;

            if (current.selected == (int)VertexButton.A)
                GUI.color = new Color(0.34f, 0.34f, 0.34f);
            if (GUILayout.Button("A", EditorStyles.miniButtonRight))
                current.selected = 4;
            GUI.color = Color.white;

            if (current.selected == (int)VertexButton.UV)
                GUI.color = new Color(0.34f, 0.34f, 0.34f);
            if (GUILayout.Button("UV", EditorStyles.miniButtonLeft))
                current.selected = 5;
            GUI.color = Color.white;

            if (current.selected == (int)VertexButton.UV2)
                GUI.color = new Color(0.34f, 0.34f, 0.34f);
            if (GUILayout.Button("UV2", EditorStyles.miniButtonMid))
                current.selected = 6;
            GUI.color = Color.white;

            if (current.selected == (int)VertexButton.UV3)
                GUI.color = new Color(0.34f, 0.34f, 0.34f);
            if (GUILayout.Button("UV3", EditorStyles.miniButtonMid))
                current.selected = 7;
            GUI.color = Color.white;


            if (current.selected == (int)VertexButton.UV4)
                GUI.color = new Color(0.34f, 0.34f, 0.34f);
            if (GUILayout.Button("UV4", EditorStyles.miniButtonRight))
                current.selected = 8;
            GUI.color = Color.white;

            current.debugMode = (DebugMode)current.selected;
            GUI.color = Color.white;

            GUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
                current.SetDebugMode();

            GUILayout.BeginVertical("box");

            if (current.selected > 0)
            {
                if (current.selected < 5)
                {
                    if (current.selected != 1)
                    {
                        switch (current.selected)
                        {
                            case 2:
                                GUILayout.Label("Edge Fluttering", EditorStyles.boldLabel);
                                EditorGUILayout.HelpBox("Help ", MessageType.Info, true);
                                break;
                            case 3:
                                GUILayout.Label("Detail Bending", EditorStyles.boldLabel);
                                EditorGUILayout.HelpBox("Help ", MessageType.Info, true);
                                break;
                            case 4:
                                GUILayout.Label("Primary Bending", EditorStyles.boldLabel);
                                EditorGUILayout.HelpBox("Help ", MessageType.Info, true);
                                break;
                        }
                        GUILayout.Space(5);

                        current.primaryBlending = EditorGUILayout.Slider("Max Value", current.primaryBlending, 0, 1);
                        current.secondaryBlending = EditorGUILayout.Slider("Min Value", current.secondaryBlending, 0, 1);
                        current.curvPart = EditorGUILayout.CurveField("Smooth Curve", current.curvPart);

                        if (GUILayout.Button("Apply", GUILayout.Width(200)))
                            current.ApplyVertexPart();
                    }
                    else
                    {
                        GUILayout.Label("Phase Variation", EditorStyles.boldLabel);
                        GUILayout.Space(5);

                        EditorGUILayout.HelpBox("Help ", MessageType.Info, true);

                        if (GUILayout.Button("Fill Black", GUILayout.Width(200)))
                            current.FillBlack();
                    }

                    

                    OnPaintInspector();
                }
                else
                {
                    fillTextureGray = CreateFillTexture(1, 1, new Color(0, 0, 0, 0.1f));
                    fillTextureDark = CreateFillTexture(1, 1, new Color(0, 0, 0, 0.5f));

                    xPanShift = 0;
                    yPanShift = 0;


                    if (current.GetComponentInChildren<MeshFilter>() != null)
                    {
                        m = current.GetComponentInChildren<MeshFilter>().sharedMesh;

                        canDrawView = true;

                        tris = m.triangles;

                        switch (current.selected)
                        {
                            case 5:
                                {
                                    if (m.uv.Length > 0 && m.uv != null)
                                        uvs = m.uv;
                                    else
                                    {
                                        canDrawView = false;
                                        EditorGUILayout.HelpBox("Mesh is not have UV. You can generate it", MessageType.None);
                                    }
                                }
                                break;
                            case 6:
                                {
                                    if (m.uv2.Length > 0 && m.uv2 != null)
                                        uvs = m.uv2;
                                    else
                                    {
                                        canDrawView = false;
                                        EditorGUILayout.HelpBox("Mesh is not have UV 2. You can generate it", MessageType.None);
                                    }
                                }
                                break;
                            case 7:
                                {
                                    if (m.uv3.Length > 0 && m.uv3 != null)
                                        uvs = m.uv3;
                                    else
                                    {
                                        canDrawView = false;
                                        EditorGUILayout.HelpBox("Mesh is not have UV 3. You can generate it", MessageType.None);
                                    }
                                }
                                break;
                            case 8:
                                {
                                    if (m.uv4.Length > 0 && m.uv4 != null)
                                        uvs = m.uv4;
                                    else
                                    {
                                        canDrawView = false;
                                        EditorGUILayout.HelpBox("Mesh is not have UV 4. You can generate it", MessageType.None);
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        canDrawView = false;
                    }

                    Event e = Event.current;

                    if (e.mousePosition.x > uvPreviewRect.x & e.mousePosition.x < uvPreviewRect.width & e.mousePosition.y > uvPreviewRect.y & e.mousePosition.y < uvPreviewRect.height)
                    {
                        mousePositionInsidePreview = true;
                    }
                    else
                    {
                        mousePositionInsidePreview = false;
                    }



                    if (canDrawView)
                    {

                        GUILayout.Box("", GUILayout.Height(512), GUILayout.ExpandWidth(true));

                        uvPreviewRect = GUILayoutUtility.GetLastRect();// new Rect(new Rect(sideSpace, ySpace + sideSpace, uvPreviewWindow.position.width - (sideSpace * 2), uvPreviewWindow.position.height - ySpace - (sideSpace * 2)));
                                                                       //uvPreviewRect = GUILayoutUtility.GetRect(512f, 512f);

                        GUI.DrawTexture(new Rect(uvPreviewRect.position.x, uvPreviewRect.position.y, uvPreviewRect.width, uvPreviewRect.height), fillTextureGray);

                        GUI.DrawTexture(uvPreviewRect, fillTextureDark);

                        if (Event.current.type == EventType.Repaint)
                        {
                            GUI.BeginClip(uvPreviewRect);
                            GL.PushMatrix();
                            //GRID


                            //UV
                            for (int i = 0; i < tris.Length; i += 3)
                            {


                                int line1x1 = (int)(uvs[tris[i]].x * (scale * windowDefaultSize) + xPanShift);
                                int line1y1 = (int)(-uvs[tris[i]].y * (scale * windowDefaultSize) + yPanShift) + windowDefaultSize;
                                int line1x2 = (int)(uvs[tris[i + 1]].x * (scale * windowDefaultSize) + xPanShift);
                                int line1y2 = (int)(-uvs[tris[i + 1]].y * (scale * windowDefaultSize) + yPanShift) + windowDefaultSize;

                                int line2x1 = (int)(uvs[tris[i + 1]].x * (scale * windowDefaultSize) + xPanShift);
                                int line2y1 = (int)(-uvs[tris[i + 1]].y * (scale * windowDefaultSize) + yPanShift) + windowDefaultSize;
                                int line2x2 = (int)(uvs[tris[i + 2]].x * (scale * windowDefaultSize) + xPanShift);
                                int line2y2 = (int)(-uvs[tris[i + 2]].y * (scale * windowDefaultSize) + yPanShift) + windowDefaultSize;

                                int line3x1 = (int)(uvs[tris[i + 2]].x * (scale * windowDefaultSize) + xPanShift);
                                int line3y1 = (int)(-uvs[tris[i + 2]].y * (scale * windowDefaultSize) + yPanShift) + windowDefaultSize;
                                int line3x2 = (int)(uvs[tris[i]].x * (scale * windowDefaultSize) + xPanShift);
                                int line3y2 = (int)(-uvs[tris[i]].y * (scale * windowDefaultSize) + yPanShift) + windowDefaultSize;


                                DrawLine(line1x1, line1y1, line1x2, line1y2, new Color(0, 1, 1, 1));
                                DrawLine(line2x1, line2y1, line2x2, line2y2, new Color(0, 1, 1, 1));
                                DrawLine(line3x1, line3y1, line3x2, line3y2, new Color(0, 1, 1, 1));

                                DrawDot(uvs[tris[i]].x * (scale * windowDefaultSize), -uvs[tris[i]].y * (scale * windowDefaultSize) + windowDefaultSize, 2f, new Color(1, 0, 0, 1));

                            }

                            Repaint();
                            GL.PopMatrix();
                            GUI.EndClip();
                        }
                        //EditorGUIUtility.AddCursorRect(uvPreviewRect, MouseCursor.Pan);

                    }

                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button("◄ Copy", EditorStyles.miniButtonLeft))
                    {
                        current.CopyUVLayer(true);
                    }

                    if (GUILayout.Button("Delete UV Layer", EditorStyles.miniButtonMid))
                    {
                        current.DeleteUVLayer();
                    }

                    if (GUILayout.Button("Copy ►", EditorStyles.miniButtonRight))
                    {
                        current.CopyUVLayer(false);
                    }

                    GUILayout.EndHorizontal();
                    if (GUILayout.Button("Update Bounds", GUILayout.Width(200)))
                    {
                        current.UpdateBounds();
                    }
                }
            }
            else
            {
                /*
                current.BendingModeSelected = (BendingModes)EditorGUILayout.EnumPopup("Primary Blending", current.BendingModeSelected);
                current.set1StBendingModes = (Set1stBendingModes)EditorGUILayout.EnumPopup("Secondary Blending", current.set1StBendingModes);
                current.mask2NdBendingModes = (Mask2ndBendingModes)EditorGUILayout.EnumPopup("Mask secondary Blending", current.mask2NdBendingModes);

                if (GUILayout.Button("Apply Settings", GUILayout.Width(200)))
                {
                    current.CreateBendingSetup();
                }
                */
                GUI.color = Color.yellow;
                if (GUILayout.Button("Create Prefab", GUILayout.Width(200)))
                {
                    current.ReSaveObject();
                }
                GUI.color = Color.white;
            }
            GUILayout.EndVertical();
            GUILayout.Space(5);
        }

        private void OnPaintInspector()
        {
            GUILayout.Space(5);
            GUILayout.Label("Paint Settings", EditorStyles.boldLabel);
            

            GUI.color = (current.isPrecisionPaint) ? Color.green : Color.white;
            if (GUILayout.Button((current.isPrecisionPaint) ? "Stop Paint" : "Paint"))
                current.isPrecisionPaint = !current.isPrecisionPaint;
            GUI.color = Color.white;

            if (current.isPrecisionPaint)
            {
                GUI.color = new Color(0.9f, 0.9f, 0.9f, 1.0f);
                GUILayout.BeginVertical("box");

                GUILayout.BeginHorizontal();
                GUI.color = Color.white;
                if (GUILayout.Button((current.leafSelectionState == LeafSelection.None) ? "Select Leaf" : "Disable", GUILayout.Width(200)))
                {
                    if (current.leafSelectionState == LeafSelection.None)
                    {
                        current.leafSelectionState = LeafSelection.Selecting;
                        selectionPoint = -1;
                    }
                    else
                        current.leafSelectionState = LeafSelection.None;
                }

                if (current.leafSelectionState == LeafSelection.Selected)
                {
                    GUI.color = Color.yellow;
                    if (GUILayout.Button("Reselect Leaf", GUILayout.Width(200)))
                    {
                        current.leafSelectionState = LeafSelection.Selecting;
                        selectionPoint = -1;
                    }
                }
                GUI.color = Color.white;
                GUILayout.EndHorizontal();

                current.roundOuterSize = EditorGUILayout.Slider("Brush Size", current.roundOuterSize, 0.01f, 2f);
                current.roundInnerSize = EditorGUILayout.Slider("Smooth Percent", current.roundInnerSize, 0f, 1f);
                GUILayout.Space(5);
                

                phaseBranchType = (PhaseBranchType)EditorGUILayout.EnumPopup("Brush Type", phaseBranchType);

                if (phaseBranchType == PhaseBranchType.Default)
                {
                    current.roundStrenght = EditorGUILayout.Slider("Brush Strenght", current.roundStrenght, 0.05f, 1f);
                    EditorGUILayout.MinMaxSlider("Brush Limit", ref current.roundStrenghtMin, ref current.roundStrenghtMax, 0, 1.0f);
                }

                if (phaseBranchType == PhaseBranchType.Discrete)
                {
                    brushPhase = EditorGUILayout.IntSlider(brushPhase, 0, 255);
                    if (current.selected == 1)
                    GUI.color = new Color((float)brushPhase / 255, 0, 0, 1);
                    else
                    if (current.selected == 2)
                        GUI.color = new Color(0, (float)brushPhase / 255, 0, 1);
                    else
                    if (current.selected == 3)
                        GUI.color = new Color(0, 0, (float)brushPhase / 255, 1);
                    else
                        if (current.selected == 4)
                        GUI.color = new Color((float)brushPhase / 255, (float)brushPhase / 255, (float)brushPhase / 255, 1);
                    GUILayout.Box("", GUILayout.ExpandWidth(true));
                    GUI.color = Color.white;
                }

                    GUILayout.EndVertical();
            }
        }

        private void OnSceneGUI()
        {
            current = (VegetationVertexTool)target;
            if (current.isPrecisionPaint)
                OnSceneGUIPrecisionPainting();
        }

        void OnSceneGUIPrecisionPainting()
        {
            if (_sceneMeshRaycaster == null)
                _sceneMeshRaycaster = new SceneMeshRaycaster();

            int controlId = GUIUtility.GetControlID(FocusType.Passive);

            switch (Event.current.type)
            {
                case EventType.Repaint:
                    {
                        current.GetComponent<MeshRenderer>().sharedMaterial.SetVector("_DebugLight", new Vector4(0f, 0f, 0f, (current.isPrecisionPaint == false) ? 0f : 1.0f));
                        if (current.leafSelectionState == LeafSelection.Selecting)
                            PrecisionSelectFirst();
                        else
                            PrecisionPaintItem(false);
                    }
                    break;
                case EventType.MouseUp:
                    {
                        HandleUtility.Repaint();
                        _painting = false;
                    }
                    break;
                case EventType.KeyDown:
                    {
                        if (Event.current.keyCode == KeyCode.LeftAlt)
                            isAltPressed = true;
                    }
                    break;
                case EventType.KeyUp:
                    {
                        if (Event.current.keyCode == KeyCode.LeftAlt)
                            isAltPressed = false;
                    }
                    break;


                case EventType.MouseMove:
                    {
                        HandleUtility.Repaint();
                        if (current.leafSelectionState == LeafSelection.Selecting)
                            PrecisionSelectFirst();
                        else
                            PrecisionPaintItem(false);
                    }
                    break;
                case EventType.MouseDrag:
                    {
                        HandleUtility.Repaint();
                        if (_painting)
                        {
                            if (current.leafSelectionState == LeafSelection.Selecting)
                                PrecisionSelectFirst();
                            else
                                PrecisionPaintItem(true);
                        }
                    }
                    break;
                case EventType.MouseDown:
                    {
                        HandleUtility.Repaint();
                        if (Event.current.button == 0)
                        {
                            _painting = true;

                            if (!isAltPressed)
                            {
                                GUIUtility.hotControl = controlId;
                                Event.current.Use();
                            }

                            if (current.leafSelectionState == LeafSelection.Selecting)
                            {
                                PrecisionSelectFirst();
                                if (selectionPoint >= 0)
                                {
                                    current.leafSelectionState = LeafSelection.Selected;
                                }
                            }
                            else
                                PrecisionPaintItem(true);
                        }
                        else
                            if (!isAltPressed)
                        {
                            GUIUtility.hotControl = 0;
                            Event.current.Use();
                        }
                    }
                    break;
                default:
                    {

                    }
                    break;
            }


        }

        void PrecisionSelectFirst()
        {

            if (_sceneMeshRaycaster == null) return;

            Mesh currentMesh = current.GetComponentInChildren<MeshFilter>().sharedMesh;
            Vector3[] points = currentMesh.vertices;
            Color32[] colors = currentMesh.colors32;
            Vector2[] uv3 = currentMesh.uv3;

            selectionPoint = -1;

            MeshRendererRaycastInfo info = new MeshRendererRaycastInfo();
            info.Mesh = current.GetComponentInChildren<MeshFilter>().sharedMesh;
            info.MeshRenderer = current.GetComponentInChildren<MeshRenderer>();
            info.LocalToWorldMatrix4X4 = info.MeshRenderer.localToWorldMatrix;
            info.Bounds = info.MeshRenderer.bounds;

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit raycastHit;
            if (_sceneMeshRaycaster.RaycastCurrenMesh(ray, out raycastHit, info))
            {


                float min = 99f;
                int ch = 0;
                for (int i = 0; i < points.Length; i++)
                {
                    Vector3 localVertex = current.transform.TransformPoint(points[i]);

                    float dist = Vector3.Distance(localVertex, raycastHit.point);
                    if (dist < min)
                    {
                        ch++;
                        selectionPoint = i;
                        min = dist;
                    }
                }


                if (selectionPoint >= 0)
                {
                    current.GetComponent<MeshRenderer>().sharedMaterial.SetVector("_DebugLight", new Vector4(1f, uv3[selectionPoint].x, uv3[selectionPoint].y, (current.isPrecisionPaint) ? 1 : 0f));
                    Vector3 globalPoint = current.transform.TransformPoint(points[selectionPoint]);
                    float size = HandleUtility.GetHandleSize(globalPoint) * 0.1f;
                    Gizmos.color = Color.black;
                    Handles.SphereHandleCap(0, globalPoint, Quaternion.identity, size, EventType.Repaint);

                    Gizmos.color = Color.blue;
                    Vector3 normal = raycastHit.normal.normalized;
                    Handles.DrawLine(globalPoint, globalPoint + normal);
                }
            }



        }

        void PrecisionPaintItem(bool addVegetationItem)
        {
            if (_sceneMeshRaycaster == null) return;

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit raycastHit;

            MeshRendererRaycastInfo info = new MeshRendererRaycastInfo();
            info.Mesh = current.GetComponentInChildren<MeshFilter>().sharedMesh;
            info.MeshRenderer = current.GetComponentInChildren<MeshRenderer>();
            info.LocalToWorldMatrix4X4 = info.MeshRenderer.localToWorldMatrix;
            info.Bounds = info.MeshRenderer.bounds;

            if (_sceneMeshRaycaster.RaycastCurrenMesh(ray, out raycastHit, info))
            {
                float size = HandleUtility.GetHandleSize(raycastHit.point) * 0.1f;

                Gizmos.color = Color.white;
                Handles.SphereHandleCap(0, raycastHit.point, Quaternion.identity, size, EventType.Repaint);

                Gizmos.color = Color.green;
                Vector3 normal = raycastHit.normal.normalized;
                Handles.DrawLine(raycastHit.point, raycastHit.point + normal);
                Gizmos.color = Color.white;
                Handles.DrawWireDisc(raycastHit.point, raycastHit.normal, current.roundOuterSize);
                Handles.DrawWireDisc(raycastHit.point, raycastHit.normal, current.roundOuterSize - current.roundOuterSize * current.roundInnerSize);

                if (isAltPressed) return;
                
                if (Event.current.control)
                {
                    PaintThis(info, raycastHit, false, addVegetationItem);
                }
                else
                {
                    PaintThis(info, raycastHit, true, addVegetationItem);
                }
            }
        }

        void PaintThis(MeshRendererRaycastInfo info, RaycastHit hit, bool input, bool addVegetationItem)
        {
            Mesh currentMesh = current.GetComponentInChildren<MeshFilter>().sharedMesh;
            Vector3[] points = currentMesh.vertices;
            Vector3[] normals = currentMesh.normals;
            Color[] colors = currentMesh.colors;
            Vector2[] uv3 = currentMesh.uv3;


            for (int i = 0; i < points.Length; i++)
            {
                if (current.leafSelectionState == LeafSelection.Selected && selectionPoint > 0)
                    if (!uv3[selectionPoint].Equals(uv3[i])) continue;

                Vector3 localVertex = current.transform.TransformPoint(points[i]);
                if (Vector3.Distance(localVertex, hit.point) >= current.roundOuterSize) continue;

                Gizmos.color = Color.green;
                Handles.SphereHandleCap(0, localVertex, Quaternion.identity, 0.01f, EventType.Repaint);

                if (!addVegetationItem) continue;
                float innerSmooth = 1.0f;

                float innerDist = Vector3.Distance(localVertex, hit.point);
                if (innerDist >= current.roundInnerSize)
                {
                    innerSmooth = 1.0f - Mathf.Clamp01((innerDist - current.roundInnerSize) / (current.roundOuterSize - current.roundInnerSize));
                }

                float strenght = current.roundStrenght * innerSmooth;
                switch (current.selected)
                {
                    case 1:
                        if (phaseBranchType == PhaseBranchType.Default)
                            colors[i].r = Mathf.Lerp(colors[i].r, (input) ? current.roundStrenghtMax : current.roundStrenghtMin, strenght);
                        else
                            colors[i].r = (float)brushPhase/255;
                        break;
                    case 2:
                        if (phaseBranchType == PhaseBranchType.Default)
                            colors[i].g = Mathf.Lerp(colors[i].g, (input) ? current.roundStrenghtMax : current.roundStrenghtMin, strenght);
                        else
                            colors[i].g = (float)brushPhase / 255;
                        break;
                    case 3:
                        if (phaseBranchType == PhaseBranchType.Default)
                            colors[i].b = Mathf.Lerp(colors[i].b, (input) ? current.roundStrenghtMax : current.roundStrenghtMin, strenght);
                        else
                            colors[i].b = (float)brushPhase / 255;
                        break;
                    case 4:
                        if (phaseBranchType == PhaseBranchType.Default)
                            colors[i].a = Mathf.Lerp(colors[i].a, (input) ? current.roundStrenghtMax : current.roundStrenghtMin, strenght);
                        else
                            colors[i].a = (float)brushPhase / 255;
                        break;
                }

            }

            if (!addVegetationItem) return;
            currentMesh.colors = colors;
            currentMesh.UploadMeshData(false);

        }

        private Texture2D CreateFillTexture(int width, int height, Color fillColor)
        {

            Texture2D texture = new Texture2D(width, height);
            Color[] pixels = new Color[width * height];

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = fillColor;
            }

            texture.SetPixels(pixels);
            texture.Apply();

            return texture;
        }



        private void DrawLine(int x1, int y1, int x2, int y2, Color lineColor)

        {
            GL.Begin(GL.LINES);
            GL.Color(lineColor);
            GL.Vertex3(x1, y1, 0);
            GL.Vertex3(x2, y2, 0);
            GL.End();
        }


        private void DrawDot(float x, float y, float radius, Color lineColor)

        {
            GL.Begin(GL.TRIANGLE_STRIP);
            GL.Color(lineColor);
            GL.Vertex3(x - radius, y - radius, 0);
            GL.Vertex3(x - radius, y + radius, 0);
            GL.Vertex3(x + radius, y + radius, 0);

            GL.Vertex3(x + radius, y + radius, 0);
            GL.Vertex3(x + radius, y - radius, 0);
            GL.Vertex3(x - radius, y - radius, 0);
            GL.End();
        }


    }
}