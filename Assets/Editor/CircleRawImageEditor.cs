﻿using UnityEditor;
using UIExtensions;
using UnityEngine;


[CustomEditor(typeof(CircleRawImage))]
public class CircleRawImageEditor : Editor
{
    private SerializedProperty radiusProperty;
    private Vector3[] points = new Vector3[EditorUtil.ELLIPSE_VERTEX_COUNT];

    void OnEnable()
    {
        radiusProperty = serializedObject.FindProperty("radius");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CircleRawImage obj = (CircleRawImage)target;
        obj.Radius = radiusProperty.floatValue;

        serializedObject.ApplyModifiedProperties();
    }

    public void OnSceneGUI()
    {
        CircleRawImage obj = (CircleRawImage)target;
        if (!obj.useCollider) return;

        Handles.color = Color.green;

        var transform = obj.transform;
        var scale = transform.localScale;
        if (Mathf.Abs(scale.x - scale.y) < float.Epsilon)
        {
            Handles.DrawWireDisc(transform.position, transform.forward.normalized, obj.Radius * scale.x);
        }
        else
        {
            float a = obj.Radius;
            float b = obj.Radius;
            EditorUtil.DrawEllipse(points, transform, a, b);
        }
    }
}