using EditorExtensions;
using UIExtensions;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PolygonImage))]
[CanEditMultipleObjects]
public class PolygonImageEditor : Editor
{
    private static Texture editBtnTex = null;
    private static Texture adjustBtnTex = null;
    private static GUIContent editBtnContent;
    private static GUIContent adjustBtnContent;
    private static GUILayoutOption[] editLabelOptions = new[] { GUILayout.Width(100f), GUILayout.Height(22f), GUILayout.ExpandWidth(false) };
    private static GUILayoutOption[] editToggleOptions = new[] { GUILayout.Width(30f), GUILayout.Height(22f), GUILayout.ExpandWidth(false) };

    private float pointRadius = 0.6f;
    private bool editting = false;
    private bool needsRepaint = false;
    private Vector2 pivot;
    private Vector3[] corners = new Vector3[4];

    private PolygonImage obj;
    private RectTransform objTrans;
    private Canvas rootCanvas = null;
    private SelectionInfo selectionInfo;


    private void Awake()
    {
        if (editBtnTex == null) 
            editBtnTex = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Editor/PolygonButton/Textures/EditBtn.png");

        if (adjustBtnTex == null)
            adjustBtnTex = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Editor/PolygonButton/Textures/AdjustBtn.png");

        editBtnContent = new GUIContent(editBtnTex);
        adjustBtnContent = new GUIContent(adjustBtnTex);
    }

    private void OnEnable()
    {
        obj = (PolygonImage)target;
        objTrans = obj.GetComponent<RectTransform>();
        UpdatePivot();

        var canvas = obj.GetComponentInParent<Canvas>();
        rootCanvas = canvas == null ? null : canvas.rootCanvas;
        selectionInfo = new SelectionInfo();
        pointRadius = obj.pointRadius * (rootCanvas == null ? 1f : rootCanvas.transform.localScale.x);

        Undo.undoRedoPerformed -= UpdatePivot;
        Undo.undoRedoPerformed += UpdatePivot;
    }

    private void OnDisable()
    {
        editting = false;
        UpdateHandles();

        Undo.undoRedoPerformed -= UpdatePivot;
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

                UpdateHandles();
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Adjust Shape", editLabelOptions);

            if (GUILayout.Button(adjustBtnContent, editToggleOptions))
            {
                Undo.RecordObject(obj, "Adjust Pivot");
                EditorUtil.OnPivotChangedAdjust(objTrans, pivot, obj.Points);
                UpdatePivot();

                needsRepaint = true;
                SceneView.RepaintAll();
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

    private void UpdatePivot()
    {
        pivot = objTrans.pivot;
    }

    private void UpdateHandles()
    {
        Tools.hidden = editting;
        objTrans.hideFlags = editting ? HideFlags.NotEditable : HideFlags.None;
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
        else if(guiEvent.modifiers == EventModifiers.Control && guiEvent.button == 0)
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

            if (newPointIndex == obj.Points.Count)
            {
                obj.Points.Add(EditorUtil.InverseTransformPoint(objTrans, corners, mousePosition));
            }
            else
            {
                obj.Points.Insert(newPointIndex, EditorUtil.InverseTransformPoint(objTrans, corners, mousePosition));
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
            obj.Points[selectionInfo.pointIndex] = EditorUtil.InverseTransformPoint(objTrans, corners, selectionInfo.positionAtStartOfDrag);
            Undo.RecordObject(obj, "Move Point");
            obj.Points[selectionInfo.pointIndex] = EditorUtil.InverseTransformPoint(objTrans, corners, mousePosition);

            selectionInfo.pointIsSelected = false;
            selectionInfo.mouseIsOverPoint = false;
            selectionInfo.pointIndex = -1;
            needsRepaint = true;
        }
    }

    private void HandleLeftMouseDrag(Vector3 mousePosition)
    {
        if (selectionInfo.pointIsSelected)
        {
            obj.Points[selectionInfo.pointIndex] = EditorUtil.InverseTransformPoint(objTrans, corners, mousePosition);
            needsRepaint = true;
        }
    }

    private void HandleControlAndLeftMouseUp()
    {
        if (!selectionInfo.mouseIsOverPoint) return;

        Undo.RecordObject(obj, "Remove Point");
        obj.Points.RemoveAt(selectionInfo.pointIndex);
    }

    private void UpdateMouseOverInfo(Vector3 mousePosition)
    {
        int mouseOverPointIndex = -1;
        pointRadius = obj.pointRadius * (rootCanvas == null ? 1f : rootCanvas.transform.localScale.x);

        for (int i = 0; i < obj.Points.Count; i++)
        {
            if (Vector3.Distance(mousePosition, objTrans.TransformPoint(obj.Points[i])) < pointRadius)
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
                var currentPoint = objTrans.TransformPoint(obj.Points[i]);
                var nextPoint = objTrans.TransformPoint(obj.Points[(i + 1) % count]);
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
            Vector3 start = objTrans.TransformPoint(obj.Points[i]);
            Vector3 end = objTrans.TransformPoint(obj.Points[(i + 1) % count]);

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
            Handles.DrawSolidDisc(objTrans.TransformPoint(obj.Points[i]), objTrans.forward, pointRadius);
        }

        needsRepaint = false;
    }

    private void Draw()
    {
        Handles.color = Color.yellow;
        var count = obj.Points.Count;

        for (int i = 0; i < obj.Points.Count; i++)
        {
            Vector3 start = objTrans.TransformPoint(obj.Points[i]);
            Vector3 end = objTrans.TransformPoint(obj.Points[(i + 1) % count]);
            Handles.DrawLine(start, end);
        }

        needsRepaint = false;
    }
}