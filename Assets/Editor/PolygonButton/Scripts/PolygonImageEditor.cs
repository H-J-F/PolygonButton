using EditorExtensions;
using UIExtensions;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PolygonImage))]
[CanEditMultipleObjects]
public class PolygonImageEditor : Editor
{
    private static Texture editBtnTex = null;
    private static GUILayoutOption[] editLabelOptions = new[] { GUILayout.Width(80f), GUILayout.Height(22f), GUILayout.ExpandWidth(false) };
    private static GUILayoutOption[] editToggleOptions = new[] { GUILayout.Width(30f), GUILayout.Height(22f), GUILayout.ExpandWidth(false) };

    private float pointRadius = 0.6f;
    private bool editting = false;
    private bool needsRepaint = false;

    private PolygonImage obj;
    private Transform objTrans;
    private Canvas rootCanvas = null;
    private SelectionInfo selectionInfo;
    private GUIContent editBtnContent = new GUIContent(editBtnTex);


    private void Awake()
    {
        if (editBtnTex == null) 
            editBtnTex = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Editor/PolygonButton/Textures/EditBtn.png");
    }

    private void OnEnable()
    {
        obj = (PolygonImage)target;
        objTrans = obj.transform;
        var canvas = obj.GetComponentInParent<Canvas>();
        rootCanvas = canvas == null ? null : canvas.rootCanvas;
        selectionInfo = new SelectionInfo();
        pointRadius = obj.pointRadius * (rootCanvas == null ? 1f : rootCanvas.transform.localScale.x);
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

        if (guiEvent.type == EventType.Repaint)
        {
            if (editting) 
                DrawWithDisc();
            else
                Draw();
        }
        else if (guiEvent.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
        else
        {
            HandleInput(guiEvent);
            if (needsRepaint)
            {
                HandleUtility.Repaint();
            }
        }

        // switch (guiEvent.type)
        // {
        //     case EventType.Repaint when editting:
        //         DrawWithDisc();
        //         break;
        //
        //     case EventType.Repaint:
        //         Draw();
        //         break;
        //
        //     case EventType.Layout:
        //         HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        //         break;
        //
        //     default:
        //         HandleInput(guiEvent);
        //         if (needsRepaint)
        //         {
        //             HandleUtility.Repaint();
        //         }
        //         break;
        // }
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

        if (!selectionInfo.pointIsSelected)
        {
            UpdateMouseOverInfo(worldPos);
        }
    }

    private void HandleLeftMouseDown(Vector3 mousePosition)
    {
        if (!selectionInfo.mouseIsOverPoint)
        {
            // TODO: 新增点
            Undo.RecordObject(obj, "Add PolygonImage Point");
            obj.Points.Add(obj.transform.InverseTransformPoint(mousePosition));
            selectionInfo.pointIndex = obj.Points.Count - 1;
        }

        selectionInfo.pointIsSelected = true;
        selectionInfo.positionAtStartOfDrag = mousePosition;
        needsRepaint = true;
    }

    private void HandleLeftMouseUp(Vector3 mousePosition)
    {
        if (selectionInfo.pointIsSelected)
        {
            obj.Points[selectionInfo.pointIndex] = objTrans.InverseTransformPoint(selectionInfo.positionAtStartOfDrag);
            Undo.RecordObject(obj, "Move Point");
            obj.Points[selectionInfo.pointIndex] = objTrans.InverseTransformPoint(mousePosition);

            selectionInfo.pointIsSelected = false;
            selectionInfo.pointIndex = -1;
            needsRepaint = true;
        }
    }

    private void HandleLeftMouseDrag(Vector3 mousePosition)
    {
        if (selectionInfo.pointIsSelected)
        {
            obj.Points[selectionInfo.pointIndex] = objTrans.InverseTransformPoint(mousePosition);
            needsRepaint = true;
        }
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
    }

    private void DrawWithDisc()
    {
        pointRadius = obj.pointRadius * (rootCanvas == null ? 1f : rootCanvas.transform.localScale.x);

        for (int i = 0; i < obj.Points.Count; i++)
        {
            Handles.color = Color.yellow;
            Vector3 start = objTrans.TransformPoint(obj.Points[i]);
            Vector3 end = objTrans.TransformPoint(obj.Points[(i + 1) % obj.Points.Count]);
            Handles.DrawDottedLine(start, end, 4);
            
            Handles.color = i != selectionInfo.pointIndex ? Color.yellow : (selectionInfo.pointIsSelected ? Color.cyan : Color.green);
            Handles.DrawSolidDisc(start, objTrans.forward, pointRadius);
        }

        needsRepaint = false;
    }

    private void Draw()
    {
        Handles.color = Color.yellow;

        for (int i = 0; i < obj.Points.Count; i++)
        {
            Vector3 start = objTrans.TransformPoint(obj.Points[i]);
            Vector3 end = objTrans.TransformPoint(obj.Points[(i + 1) % obj.Points.Count]);
            Handles.DrawLine(start, end);
        }

        needsRepaint = false;
    }
}