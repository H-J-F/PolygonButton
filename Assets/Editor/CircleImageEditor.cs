using UnityEditor;
using UIExtensions;
using UnityEngine;


[CustomEditor(typeof(CircleImage))]
public class CircleImageEditor : Editor
{
    private SerializedProperty radiusProperty;

    void OnEnable()
    {
        radiusProperty = serializedObject.FindProperty("radius");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CircleImage obj = (CircleImage) target;
        obj.Radius = radiusProperty.floatValue;

        serializedObject.ApplyModifiedProperties();
    }

    public void OnSceneGUI()
    {
        CircleImage obj = (CircleImage) target;
        if (!obj.useCollider) return;

        Handles.color = Color.green;

        var transform = obj.transform;
        Handles.DrawWireDisc(transform.position, transform.forward.normalized, obj.Radius);
    }
}