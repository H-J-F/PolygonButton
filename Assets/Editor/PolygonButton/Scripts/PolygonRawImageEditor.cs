using System.Collections.Generic;
using EditorExtensions;
using UIExtensions;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PolygonRawImage))]
[CanEditMultipleObjects]
public class PolygonRawImageEditor : Editor
{
    private static Texture editBtnTex = null;
    private static GUILayoutOption[] editLabelOptions = new[] { GUILayout.Width(80f), GUILayout.Height(22f), GUILayout.ExpandWidth(false) };
    private static GUILayoutOption[] editToggleOptions = new[] { GUILayout.Width(30f), GUILayout.Height(22f), GUILayout.ExpandWidth(false) };

    private float pointRadius = 0.6f;
    private bool editting = false;
    private bool needsRepaint = false;
    private List<Vector3> points3D = new List<Vector3>();

    private PolygonRawImage obj;
    private Transform objTrans;
    private Canvas rootCanvas = null;
    private SelectionInfo selectionInfo;
    private GUIContent editBtnContent;


    private void Awake()
    {
        if (editBtnTex == null)
            editBtnTex = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Editor/PolygonButton/Textures/EditBtn.png");

        editBtnContent = new GUIContent(editBtnTex);
    }

    private void OnEnable()
    {
        obj = (PolygonRawImage)target;
        objTrans = obj.transform;
        var canvas = obj.GetComponentInParent<Canvas>();
        rootCanvas = canvas == null ? null : canvas.rootCanvas;
        selectionInfo = new SelectionInfo();
        pointRadius = obj.pointRadius * (rootCanvas == null ? 1f : rootCanvas.transform.localScale.x);

        Undo.undoRedoPerformed -= InitPoints3D;
        Undo.undoRedoPerformed += InitPoints3D;

        InitPoints3D();
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Edit Shape", editLabelOptions);
            bool tempBool = GUILayout.Toggle(editting, editBtnContent, "Button", editToggleOptions);
            if (tempBool != editting)
            {
                SceneView.RepaintAll();
                editting = tempBool;
                needsRepaint = true;
            }
        }
        EditorGUILayout.EndHorizontal();

        base.OnInspectorGUI();

        serializedObject.ApplyModifiedProperties();
    }

    public void OnSceneGUI()
    {
        Event guiEvent = Event.current;

        switch (guiEvent.type)
        {
            case EventType.Repaint when editting:
                DrawWithDisc();
                break;

            case EventType.Repaint:
                Draw();
                break;

            case EventType.Layout:
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                break;

            default:
                HandleInput(guiEvent);
                if (needsRepaint) HandleUtility.Repaint();
                break;
        }
    }

    private void InitPoints3D()
    {
        for (var i = 0; i < obj.Points.Count; i++)
        {
            var point = objTrans.TransformPoint(obj.Points[i]);
            if (i >= points3D.Count)
            {
                points3D.Add(point);
            }
            else
            {
                points3D[i] = point;
            }
        }

        for (int i = points3D.Count - 1; i >= obj.Points.Count; i--)
        {
            points3D.RemoveAt(i);
        }
    }

    private void HandleInput(Event guiEvent)
    {
        if (!editting) return;

        Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        Plane plane = new Plane(objTrans.forward, objTrans.position);
        plane.Raycast(mouseRay, out var enter);
        Vector3 worldPos = mouseRay.GetPoint(enter);

        if (guiEvent.modifiers == EventModifiers.None && guiEvent.button == 0)
        {
            switch (guiEvent.type)
            {
                case EventType.MouseDown:
                    HandleLeftMouseDown(worldPos);
                    break;

                case EventType.MouseUp:
                    HandleLeftMouseUp(worldPos);
                    break;

                case EventType.MouseDrag:
                    HandleLeftMouseDrag(worldPos);
                    break;
            }
        }
        else if (guiEvent.modifiers == EventModifiers.Control && guiEvent.button == 0)
        {
            if (guiEvent.type == EventType.MouseUp)
            {
                HandleControlAndLeftMouseUp();
            }
        }

        if (!selectionInfo.pointIsSelected)
        {
            UpdateMouseOverInfo(worldPos);
        }
    }

    private void HandleLeftMouseDown(Vector3 mousePosition)
    {
        if (!selectionInfo.mouseIsOverPoint)
        {
            int newPointIndex = selectionInfo.mouseIsOverLine ? selectionInfo.lineIndex + 1 : obj.Points.Count;
            Undo.RecordObject(obj, "Add PolygonImage Point");
            selectionInfo.pointIndex = newPointIndex;
            selectionInfo.mouseIsOverPoint = true;
            selectionInfo.lineIndex = -1;
            // selectionInfo.mouseIsOverLine = false; Auto call in UpdateMouseOverInfo

            if (newPointIndex == obj.Points.Count)
            {
                EditorUtil.AddMappingPoint(objTrans, obj.Points, points3D, mousePosition);
            }
            else
            {
                EditorUtil.InsertMappingPoint(objTrans, obj.Points, points3D, newPointIndex, mousePosition);
            }
        }

        selectionInfo.pointIsSelected = true;
        selectionInfo.positionAtStartOfDrag = mousePosition;
        needsRepaint = true;
    }

    private void HandleLeftMouseUp(Vector3 mousePosition)
    {
        if (selectionInfo.pointIsSelected)
        {
            EditorUtil.UpdateMappingPoint(objTrans, obj.Points, points3D, selectionInfo.pointIndex, selectionInfo.positionAtStartOfDrag);
            Undo.RecordObject(obj, "Move Point");
            EditorUtil.UpdateMappingPoint(objTrans, obj.Points, points3D, selectionInfo.pointIndex, mousePosition);

            selectionInfo.pointIsSelected = false;
            selectionInfo.pointIndex = -1;
            needsRepaint = true;
        }
    }

    private void HandleLeftMouseDrag(Vector3 mousePosition)
    {
        if (selectionInfo.pointIsSelected)
        {
            EditorUtil.UpdateMappingPoint(objTrans, obj.Points, points3D, selectionInfo.pointIndex, mousePosition);
            needsRepaint = true;
        }
    }

    private void HandleControlAndLeftMouseUp()
    {
        // only delete one point
        if (!selectionInfo.mouseIsOverPoint) return;

        Undo.RecordObject(obj, "Remove Point");
        EditorUtil.RemoveMappingPoint(obj.Points, points3D, selectionInfo.pointIndex);
    }

    private void UpdateMouseOverInfo(Vector3 mousePosition)
    {
        int mouseOverPointIndex = -1;
        pointRadius = obj.pointRadius * (rootCanvas == null ? 1f : rootCanvas.transform.localScale.x);

        for (int i = 0; i < obj.Points.Count; i++)
        {
            if (Vector3.Distance(mousePosition, points3D[i]) < pointRadius)
            {
                mouseOverPointIndex = i;
                break;
            }
        }

        if (mouseOverPointIndex != selectionInfo.pointIndex)
        {
            selectionInfo.pointIndex = mouseOverPointIndex;
            selectionInfo.mouseIsOverPoint = mouseOverPointIndex != -1;

            needsRepaint = true;
        }

        if (selectionInfo.mouseIsOverPoint)
        {
            selectionInfo.mouseIsOverLine = false;
            selectionInfo.lineIndex = -1;
        }
        else
        {
            var count = obj.Points.Count;
            int mouseOverLineIndex = -1;
            float closestLineDst = pointRadius;

            for (int i = 0; i < count; i++)
            {
                var currentPoint = points3D[i];
                var nextPoint = points3D[(i + 1) % count];
                float distFromMouseToLine = HandleUtility.DistancePointLine(mousePosition, currentPoint, nextPoint);

                if (distFromMouseToLine < closestLineDst)
                {
                    closestLineDst = distFromMouseToLine;
                    mouseOverLineIndex = i;
                }
            }

            if (selectionInfo.lineIndex != mouseOverLineIndex)
            {
                selectionInfo.lineIndex = mouseOverLineIndex;
                selectionInfo.mouseIsOverLine = mouseOverLineIndex != -1;
                needsRepaint = true;
            }
        }
    }

    private void DrawWithDisc()
    {
        pointRadius = obj.pointRadius * (rootCanvas == null ? 1f : rootCanvas.transform.localScale.x);
        var count = obj.Points.Count;

        for (int i = 0; i < count; i++)
        {
            Vector3 start = points3D[i];
            Vector3 end = points3D[(i + 1) % count];

            Handles.color = i == selectionInfo.lineIndex ? Color.green : Color.yellow;
            if (i == selectionInfo.lineIndex)
            {
                Handles.DrawLine(start, end);
            }
            else
            {
                Handles.DrawDottedLine(start, end, 4);
            }
        }

        for (int i = 0; i < count; i++)
        {
            Handles.color = i != selectionInfo.pointIndex ? Color.yellow : (selectionInfo.pointIsSelected ? Color.cyan : Color.green);
            Handles.DrawSolidDisc(points3D[i], objTrans.forward, pointRadius);
        }

        needsRepaint = false;
    }

    private void Draw()
    {
        Handles.color = Color.yellow;
        var count = obj.Points.Count;

        for (int i = 0; i < obj.Points.Count; i++)
        {
            Vector3 start = points3D[i];
            Vector3 end = points3D[(i + 1) % count];
            Handles.DrawLine(start, end);
        }

        needsRepaint = false;
    }
}