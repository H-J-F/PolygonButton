using UnityEditor;
using UIExtensions;
using UnityEngine;


[CustomEditor(typeof(CircleImage))]
[CanEditMultipleObjects]
public class CircleImageEditor : Editor
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

        CircleImage obj = (CircleImage) target;
        obj.Radius = radiusProperty.floatValue;

        serializedObject.ApplyModifiedProperties();
    }

    public void OnSceneGUI()
    {
        CircleImage obj = (CircleImage) target;
        if (!obj.useIrregularRaycast) return;

        Handles.color = Color.green;

        var transform = obj.transform;
        var scale = transform.localScale;

        if (Mathf.Abs(scale.x - scale.y) < float.Epsilon)
        {
            Handles.DrawWireDisc(transform.position, transform.forward.normalized, obj.Radius * scale.x);
        }
        else
        {
            EditorUtil.DrawEllipse(points, transform, obj.Radius, obj.Radius);
        }
    }
}