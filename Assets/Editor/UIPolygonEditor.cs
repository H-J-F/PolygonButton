using UnityEditor;
using UIExtensions;
using UnityEngine;


[CustomEditor(typeof(UIPolygon))]
public class UIPolygonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UIPolygon obj = (UIPolygon)target;
        if (GUILayout.Button("Adapt Polygon"))
        {
            obj.AdaptPolygon();
        }
    }
}