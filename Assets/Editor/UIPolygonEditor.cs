using UnityEditor;
using UIExtensions;
using UnityEngine;


[CustomEditor(typeof(UIPolygon))]
public class UIPolygonEditor : Editor
{
    private Vector3[] points3D;


    public void OnSceneGUI()
    {
        UIPolygon obj = (UIPolygon) target;
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
            points3D[i] = rectTransform.position + (Vector3) (rectTransform.localToWorldMatrix * points[i % points.Length]);
        }
    }
}