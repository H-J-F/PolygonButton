using UIExtensions;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PolygonImage))]
public class PolygonImageEditor : Editor
{
    private SerializedProperty pointsProperty;
    private Vector3[] points3D;


    private void OnEnable()
    {
        pointsProperty = serializedObject.FindProperty("points");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PolygonImage obj = (PolygonImage)target;

        if (pointsProperty.arraySize < 3)
        {
            Undo.PerformUndo();
        }

        serializedObject.ApplyModifiedProperties();
    }

    public void OnSceneGUI()
    {
        PolygonImage obj = (PolygonImage) target;
        if (!obj.useCollider) return;

        Handles.color = Color.green;

        GetPoints(obj.Points, obj.rectTransform);
        Handles.DrawAAPolyLine(points3D);
    }

    private void GetPoints(Vector2[] points, RectTransform rectTransform)
    {
        if (points3D == null || points3D.Length != points.Length)
            points3D = new Vector3[points.Length + 1];

        for (var i = 0; i < points3D.Length; i++)
        {
            points3D[i] = rectTransform.position + (Vector3)(rectTransform.localToWorldMatrix * points[i % points.Length]);
        }
    }
}