using UnityEditor;
using UIExtensions;
using UnityEngine;


[CustomEditor(typeof(UIPolygon))]
[CanEditMultipleObjects]
public class UIPolygonEditor : Editor
{
    private UIPolygon obj;
    private Vector3[] points3D;


    private void OnEnable()
    {
        obj = (UIPolygon)target;
    }

    public void OnSceneGUI()
    {
        if (!obj.useIrregularRaycast) return;

        Handles.color = Color.yellow;

        GetPoints(obj.Points, obj.rectTransform);
        Handles.DrawAAPolyLine(points3D);
    }

    private void GetPoints(Vector2[] points, RectTransform rectTransform)
    {
        if (points3D == null || points3D.Length != points.Length)
            points3D = new Vector3[points.Length + 1];

        for (var i = 0; i < points3D.Length; i++)
        {
            points3D[i] = rectTransform.position + (Vector3) (rectTransform.localToWorldMatrix * points[i % points.Length]);
        }
    }
}