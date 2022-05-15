﻿using UIExtensions;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PolygonRawImage))]
[CanEditMultipleObjects]
public class PolygonRawImageEditor : Editor
{
    private bool hadSetPoints = false;
    private PolygonRawImage obj;
    private SerializedProperty pointsProperty;
    private Vector3[] points3D;


    private void OnEnable()
    {
        obj = (PolygonRawImage)target;
        pointsProperty = serializedObject.FindProperty("points");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (pointsProperty.arraySize < 3)
        {
            Undo.PerformUndo();
        }

        serializedObject.ApplyModifiedProperties();
    }

    public void OnSceneGUI()
    {
        PolygonRawImage obj = (PolygonRawImage)target;

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