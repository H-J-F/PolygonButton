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

    private float pointWidth = 0.6f;
    private bool editting = false;
    private bool needsRepaint = false;

    private PolygonImage obj;
    private Transform objTrans;
    private Canvas rootCanvas = null;
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

        // if (guiEvent.type == EventType.Repaint)
        // {
        //     if (editting) 
        //         DrawWithDisc();
        //     else
        //         Draw();
        //     
        // }
        // else if (guiEvent.type == EventType.Layout)
        // {
        //     HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        // }
        // else
        // {
        //     HandleInput(guiEvent);
        // }

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
                if (needsRepaint)
                {
                    HandleUtility.Repaint();
                }
                break;
        }
    }

    private void HandleInput(Event guiEvent)
    {
        if (editting && guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
        {
            needsRepaint = true;
            Undo.RecordObject(obj, "Add PolygonImage Point");

            Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
            Plane plane = new Plane(objTrans.forward, objTrans.position);
            plane.Raycast(mouseRay, out var enter);
            Vector3 worldPos = mouseRay.GetPoint(enter);

            obj.Points.Add(obj.transform.InverseTransformPoint(worldPos));
        }
    }

    private void DrawWithDisc()
    {
        Handles.color = Color.green;
        float width = pointWidth * (rootCanvas == null ? 1f : rootCanvas.transform.localScale.x);

        for (int i = 0; i < obj.Points.Count; i++)
        {
            Vector3 start = objTrans.TransformPoint(obj.Points[i]);
            Vector3 end = objTrans.TransformPoint(obj.Points[(i + 1) % obj.Points.Count]);
            Handles.DrawLine(start, end);
            Handles.DrawSolidDisc(start, objTrans.forward, width);
        }

        needsRepaint = false;
    }

    private void Draw()
    {
        Handles.color = Color.green;

        for (int i = 0; i < obj.Points.Count; i++)
        {
            Vector3 start = objTrans.TransformPoint(obj.Points[i]);
            Vector3 end = objTrans.TransformPoint(obj.Points[(i + 1) % obj.Points.Count]);
            Handles.DrawLine(start, end);
        }

        needsRepaint = false;
    }
}