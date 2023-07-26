using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sebastian.Geometry;
using UnityEditor.Build;
using UnityEngine.SceneManagement;
using Assets.Src.Aspects.RegionsScenicObjects;
using SharedCode.Aspects.Regions;
using System.IO;
using System.Text;
using Assets.Src.WorldSpace;
using UnityEditor.Build.Reporting;
using Assets.Src.ResourceSystem;
using SharedCode.Entities.GameObjectEntities;

[CustomEditor(typeof(ShapeCreator))]
public partial class ShapeEditor : Editor
{

    ShapeCreator shapeCreator;
    SelectionInfo selectionInfo;
    bool shapeChangedSinceLastRepaint;

    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();
        string helpMessage = "Left click to add points.\nShift-left click on point to delete.\nShift-left clic k on empty space to create new shape.";
        EditorGUILayout.HelpBox(helpMessage, MessageType.Info);
        Tools.hidden = EditorGUILayout.Toggle("EditMode", Tools.hidden);
        int shapeDeleteIndex = -1;
        shapeCreator.showShapesList = EditorGUILayout.Foldout(shapeCreator.showShapesList, "Show Shapes List");
        if (shapeCreator.showShapesList)
        {
            for (int i = 0; i < shapeCreator.shapes.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Shape " + (i + 1));

                GUI.enabled = i != selectionInfo.selectedShapeIndex;
                if (GUILayout.Button("Select"))
                {
                    selectionInfo.selectedShapeIndex = i;
                }
                GUI.enabled = true;

                if (GUILayout.Button("Delete"))
                {
                    shapeDeleteIndex = i;
                }
                GUILayout.EndHorizontal();
            }
        }

        if (shapeDeleteIndex != -1)
        {
            Undo.RegisterCompleteObjectUndo(shapeCreator, "Delete shape");
            shapeCreator.shapes.RemoveAt(shapeDeleteIndex);
            selectionInfo.selectedShapeIndex = Mathf.Clamp(selectionInfo.selectedShapeIndex, 0, shapeCreator.shapes.Count - 1);
        }
        if (Tools.hidden)
            if (GUI.changed)
            {
                shapeChangedSinceLastRepaint = true;
                SceneView.RepaintAll();
            }
    }

    void OnSceneGUI()
    {
        Event guiEvent = Event.current;

        if (guiEvent.type == EventType.Repaint)
        {
            Draw();
        }
        else if (guiEvent.type == EventType.Layout)
        {
            if (!Tools.hidden)
                return;
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
        else
        {
            if (!Tools.hidden)
                return;
            HandleInput(guiEvent);
            EditorUtility.SetDirty(shapeCreator);
            if (shapeChangedSinceLastRepaint)
            {
                HandleUtility.Repaint();
            }
        }
    }

    void CreateNewShape()
    {
        Undo.RegisterCompleteObjectUndo(shapeCreator, "Create shape");
        shapeCreator.shapes.Add(new Shape());
        selectionInfo.selectedShapeIndex = shapeCreator.shapes.Count - 1;
    }

    void CreateNewPoint(Vector3 position)
    {
        bool mouseIsOverSelectedShape = selectionInfo.mouseOverShapeIndex == selectionInfo.selectedShapeIndex;
        int newPointIndex = (selectionInfo.mouseIsOverLine && mouseIsOverSelectedShape) ? selectionInfo.lineIndex + 1 : SelectedShape.points.Count;
        Undo.RegisterCompleteObjectUndo(shapeCreator, "Add point" + newPointIndex);
        Undo.FlushUndoRecordObjects();
        SelectedShape.points.Insert(newPointIndex, position);
        selectionInfo.pointIndex = newPointIndex;
        selectionInfo.mouseOverShapeIndex = selectionInfo.selectedShapeIndex;
        shapeChangedSinceLastRepaint = true;
        SelectPointUnderMouse();
    }

    void DeletePointUnderMouse()
    {
        Undo.RegisterCompleteObjectUndo(shapeCreator, "Delete point");
        SelectedShape.points.RemoveAt(selectionInfo.pointIndex);
        selectionInfo.pointIsSelected = false;
        selectionInfo.mouseIsOverPoint = false;
        shapeChangedSinceLastRepaint = true;
    }

    void SelectPointUnderMouse()
    {
        selectionInfo.pointIsSelected = true;
        selectionInfo.mouseIsOverPoint = true;
        selectionInfo.mouseIsOverLine = false;
        selectionInfo.lineIndex = -1;

        selectionInfo.positionAtStartOfDrag = SelectedShape.points[selectionInfo.pointIndex];
        shapeChangedSinceLastRepaint = true;
    }

    void SelectShapeUnderMouse()
    {
        if (selectionInfo.mouseOverShapeIndex != -1)
        {
            selectionInfo.selectedShapeIndex = selectionInfo.mouseOverShapeIndex;
            shapeChangedSinceLastRepaint = true;
        }
    }

    void HandleInput(Event guiEvent)
    {
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        float drawPlaneHeight = ((ShapeCreator)target).gameObject.transform.position.y;
        float dstToDrawPlane = (drawPlaneHeight - mouseRay.origin.y) / mouseRay.direction.y;
        Vector3 mousePosition = mouseRay.GetPoint(dstToDrawPlane);
        mousePosition = ((ShapeCreator)target).gameObject.transform.InverseTransformPoint(mousePosition);
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.Shift)
        {
            HandleShiftLeftMouseDown(mousePosition);
        }

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
        {
            HandleLeftMouseDown(mousePosition);
        }

        if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0)
        {
            HandleLeftMouseUp(mousePosition);
        }

        if (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
        {
            HandleLeftMouseDrag(mousePosition);
        }

        if (!selectionInfo.pointIsSelected)
        {
            UpdateMouseOverInfo(mousePosition);
        }

    }

    void HandleShiftLeftMouseDown(Vector3 mousePosition)
    {
        if (selectionInfo.mouseIsOverPoint)
        {
            SelectShapeUnderMouse();
            DeletePointUnderMouse();
        }
        else
        {
            var g = Undo.GetCurrentGroup();
            CreateNewShape();
            CreateNewPoint(mousePosition);
            Undo.CollapseUndoOperations(g);
        }
    }

    void HandleLeftMouseDown(Vector3 mousePosition)
    {
        if (shapeCreator.shapes.Count == 0)
        {
            CreateNewShape();
        }

        SelectShapeUnderMouse();

        if (selectionInfo.mouseIsOverPoint)
        {
            SelectPointUnderMouse();
        }
        else
        {
            CreateNewPoint(mousePosition);
        }
    }

    void HandleLeftMouseUp(Vector3 mousePosition)
    {
        if (selectionInfo.pointIsSelected)
        {
            SelectedShape.points[selectionInfo.pointIndex] = selectionInfo.positionAtStartOfDrag;
            Undo.RegisterCompleteObjectUndo(shapeCreator, "Move point");
            SelectedShape.points[selectionInfo.pointIndex] = mousePosition;

            selectionInfo.pointIsSelected = false;
            selectionInfo.pointIndex = -1;
            shapeChangedSinceLastRepaint = true;
        }

    }

    void HandleLeftMouseDrag(Vector3 mousePosition)
    {
        if (selectionInfo.pointIsSelected)
        {
            SelectedShape.points[selectionInfo.pointIndex] = mousePosition;
            shapeChangedSinceLastRepaint = true;
        }

    }

    void UpdateMouseOverInfo(Vector3 mousePosition)
    {
        int mouseOverPointIndex = -1;
        int mouseOverShapeIndex = -1;
        for (int shapeIndex = 0; shapeIndex < shapeCreator.shapes.Count; shapeIndex++)
        {
            Shape currentShape = shapeCreator.shapes[shapeIndex];

            for (int i = 0; i < currentShape.points.Count; i++)
            {
                if (Vector3.Distance(mousePosition, currentShape.points[i]) < shapeCreator.handleRadius)
                {
                    mouseOverPointIndex = i;
                    mouseOverShapeIndex = shapeIndex;
                    break;
                }
            }
        }

        if (mouseOverPointIndex != selectionInfo.pointIndex || mouseOverShapeIndex != selectionInfo.mouseOverShapeIndex)
        {
            selectionInfo.mouseOverShapeIndex = mouseOverShapeIndex;
            selectionInfo.pointIndex = mouseOverPointIndex;
            selectionInfo.mouseIsOverPoint = mouseOverPointIndex != -1;

            shapeChangedSinceLastRepaint = true;
        }

        if (selectionInfo.mouseIsOverPoint)
        {
            selectionInfo.mouseIsOverLine = false;
            selectionInfo.lineIndex = -1;
        }
        else
        {
            int mouseOverLineIndex = -1;
            float closestLineDst = shapeCreator.handleRadius;
            for (int shapeIndex = 0; shapeIndex < shapeCreator.shapes.Count; shapeIndex++)
            {
                Shape currentShape = shapeCreator.shapes[shapeIndex];

                for (int i = 0; i < currentShape.points.Count; i++)
                {
                    Vector3 nextPointInShape = currentShape.points[(i + 1) % currentShape.points.Count];
                    float dstFromMouseToLine = HandleUtility.DistancePointToLineSegment(mousePosition.ToXZ(), currentShape.points[i].ToXZ(), nextPointInShape.ToXZ());
                    if (dstFromMouseToLine < closestLineDst)
                    {
                        closestLineDst = dstFromMouseToLine;
                        mouseOverLineIndex = i;
                        mouseOverShapeIndex = shapeIndex;
                    }
                }
            }

            if (selectionInfo.lineIndex != mouseOverLineIndex || mouseOverShapeIndex != selectionInfo.mouseOverShapeIndex)
            {
                selectionInfo.mouseOverShapeIndex = mouseOverShapeIndex;
                selectionInfo.lineIndex = mouseOverLineIndex;
                selectionInfo.mouseIsOverLine = mouseOverLineIndex != -1;
                shapeChangedSinceLastRepaint = true;
            }
        }
    }

    void Draw()
    {
        for (int shapeIndex = 0; shapeIndex < shapeCreator.shapes.Count; shapeIndex++)
        {
            Shape shapeToDraw = shapeCreator.shapes[shapeIndex];
            bool shapeIsSelected = shapeIndex == selectionInfo.selectedShapeIndex;
            bool mouseIsOverShape = shapeIndex == selectionInfo.mouseOverShapeIndex;
            Color deselectedShapeColour = Color.grey;

            for (int i = 0; i < shapeToDraw.points.Count; i++)
            {
                Vector3 nextPoint = shapeToDraw.points[(i + 1) % shapeToDraw.points.Count];
                nextPoint = ((ShapeCreator)target).transform.TransformPoint(nextPoint);

                Vector3 curPoint = ((ShapeCreator)target).transform.TransformPoint(shapeToDraw.points[i]);
                if (i == selectionInfo.lineIndex && mouseIsOverShape)
                {
                    Handles.color = Color.red;
                    Handles.DrawLine(curPoint, nextPoint);
                }
                else
                {
                    Handles.color = (shapeIsSelected) ? Color.black : deselectedShapeColour;
                    Handles.DrawDottedLine(curPoint, nextPoint, 4);
                }

                if (i == selectionInfo.pointIndex && mouseIsOverShape)
                {
                    Handles.color = (selectionInfo.pointIsSelected) ? Color.black : Color.red;
                }
                else
                {
                    Handles.color = (shapeIsSelected) ? Color.white : deselectedShapeColour;
                }
                Handles.DrawSolidDisc(curPoint, Vector3.up, shapeCreator.handleRadius);
            }
        }

        if (shapeChangedSinceLastRepaint)
        {
            shapeCreator.UpdateMeshDisplay();
        }

        shapeChangedSinceLastRepaint = false;
    }

    void OnEnable()
    {
        shapeChangedSinceLastRepaint = true;
        shapeCreator = target as ShapeCreator;
        selectionInfo = new SelectionInfo();
        Undo.undoRedoPerformed += OnUndoOrRedo;
        //Tools.hidden = true;
    }

    void OnDisable()
    {
        Undo.undoRedoPerformed -= OnUndoOrRedo;
        //Tools.hidden = false;
    }

    void OnUndoOrRedo()
    {
        if (selectionInfo.selectedShapeIndex >= shapeCreator.shapes.Count || selectionInfo.selectedShapeIndex == -1)
        {
            selectionInfo.selectedShapeIndex = shapeCreator.shapes.Count - 1;
        }
        shapeChangedSinceLastRepaint = true;
    }

    Shape SelectedShape
    {
        get
        {
            return shapeCreator.shapes[selectionInfo.selectedShapeIndex];
        }
    }

    public class SelectionInfo
    {
        public int selectedShapeIndex;
        public int mouseOverShapeIndex;

        public int pointIndex = -1;
        public bool mouseIsOverPoint;
        public bool pointIsSelected;
        public Vector3 positionAtStartOfDrag;

        public int lineIndex = -1;
        public bool mouseIsOverLine;
    }
    public class PostProcessProduceShapeDefs : IProcessSceneWithReport
    {
        public int callbackOrder => 0;
        static List<Vector3> _vertices = new List<Vector3>();
        static List<int> _indices = new List<int>();
        static List<Triangle> _triangles = new List<Triangle>();
        public void OnProcessScene(Scene scene, BuildReport report)
        {
            //var debugTimer = new System.Diagnostics.Stopwatch();
            //debugTimer.Start();

            string dir = AssureDir(scene);
            foreach (var rootGo in scene.GetRootGameObjects())
            {
                var shapedRegions = rootGo.GetComponentsInChildren<PolygonRegion>();
                foreach (var shapedRegion in shapedRegions)
                {
                    /*if(VisualKiller.Enabled)
                    {
                        DestroyImmediate(shapedRegion.GetComponent<ShapeCreator>());
                        DestroyImmediate(shapedRegion.GetComponent<MeshRenderer>());
                        DestroyImmediate(shapedRegion.GetComponent<MeshFilter>());
                    }
                    else*/
                    {
                        (shapedRegion.GetComponent<ShapeCreator>()).enabled = false;
                        (shapedRegion.GetComponent<MeshRenderer>()).enabled = false;
                    }
                }
            }

            //Until I come up and implement a proper way to automate cleaning up generated regions they should be manually cleaned up from time to time
            //This is no issue as the files are small. Just delete dir and restart.
            /*if (Directory.Exists(dir))
            {
                var files = Directory.GetFiles(dir, "*.json");
                foreach (var file in files)
                {
                    if (!_regions.Contains(Path.GetFileNameWithoutExtension(file).ToLower()))
                    {
                        File.Delete(file);
                    }
                }
            }*/

            //var seconds = (debugTimer?.ElapsedMilliseconds ?? 0) / 1000.0f;
            //Debug.LogError($"PostProcessProduceShapeDefs.ScenePostProcess, time: {seconds} sec, scene: {scene.name}");
        }

        public static ConcaveShapeDef ExtractConcaveShapeDef(PolygonRegion shapedRegion)
        {
            _triangles.Clear();
            ConcaveShapeDef def = new ConcaveShapeDef();
            var mesh = shapedRegion.GetComponent<ShapeCreator>().ComputedMesh;
            if(mesh == null)
                Debug.LogError($"NO MESH {shapedRegion.name}");
            _vertices.Clear();
            mesh.GetVertices(_vertices);
            _indices.Clear();
            mesh.GetIndices(_indices, 0);
            for (int i = 0; i < _indices.Count; i += 3) // used triangulator guaranties vertices order (in threes)
            {
                Triangle triangle = new Triangle
                {
                    PointA = shapedRegion.transform.TransformPoint(_vertices[_indices[i]]).ToXZ().ToShared(),
                    PointB = shapedRegion.transform.TransformPoint(_vertices[_indices[i + 1]]).ToXZ().ToShared(),
                    PointC = shapedRegion.transform.TransformPoint(_vertices[_indices[i + 2]]).ToXZ().ToShared()
                };
                _triangles.Add(triangle);
            }
            var iv = shapedRegion.transform.TransformPoint(_vertices[0]);
            float minX = iv.x;
            float maxX = iv.x;
            float minY = iv.z;
            float maxY = iv.z;
            for (int i = 1; i < _vertices.Count; i++)
            {
                var q = shapedRegion.transform.TransformPoint(_vertices[i]);
                minX = Mathf.Min(q.x, minX);
                maxX = Mathf.Max(q.x, maxX);
                minY = Mathf.Min(q.z, minY);
                maxY = Mathf.Max(q.z, maxY);
            }
            def.BoundingBox = new PrecalculatedAABB() { MinX = minX, MaxX = maxX, MinY = minY, MaxY = maxY };
            def.Triangles = _triangles.ToArray();
            def.StartHeight = shapedRegion.MinHeight;
            def.Height = shapedRegion.MaxHeight;
            return def;
        }

        static StringBuilder builder = new StringBuilder();
        public static string GetPathForGo(PolygonRegion shapedRegion)
        {
            builder.Clear();
            Transform cur = shapedRegion.transform;
            while (cur != null)
            {
                builder.Append(cur.gameObject.name);
                builder.Append("_");
                cur = cur.parent;
            }
            return builder.ToString();
        }

        public static string AssureDir(Scene scene)
        {
            return Path.Combine(Application.dataPath, "ScenicRegions/" + scene.name);
        }
    }



}
