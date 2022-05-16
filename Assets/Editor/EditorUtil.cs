using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


public static class EditorUtil
{
    private static MethodInfo extractArrayFromListMethod = null;
    public const int ELLIPSE_VERTEX_COUNT = 90;


    // 仅用于z轴不旋转的椭圆
    public static void DrawEllipse(Vector3[] points, Transform transform, float a, float b)
    {
        float degree = 360f / ELLIPSE_VERTEX_COUNT;
        var position = transform.position;
        var matrix = transform.localToWorldMatrix;

        for (int i = 0; i < ELLIPSE_VERTEX_COUNT / 2; i++)
        {
            float angle = i * degree;
            float rad = Mathf.Deg2Rad * angle;
            float tan = Mathf.Tan(rad);
            float absX = Mathf.Sqrt(a * a * b * b / (b * b + a * a * tan * tan));
            bool isAtZero = i == ELLIPSE_VERTEX_COUNT / 4;
            float x = i > ELLIPSE_VERTEX_COUNT / 4 ? -absX : absX;
            float y = isAtZero ? b : x * tan;

            points[i] = position + (Vector3) (matrix * new Vector3(x, y));
            points[points.Length - i - 1] = position + (Vector3) (matrix * new Vector3(x, -y));
        }

        Handles.DrawPolyLine(points);
    }

    public static T[] ExtractArrayFromListT<T>(List<T> list)
    {
        if (extractArrayFromListMethod == null)
        {
            var type = Assembly.Load("UnityEngine").GetType("UnityEngine.NoAllocHelpers");
            var method = type.GetMethod("ExtractArrayFromListT", BindingFlags.Static | BindingFlags.Public);
            extractArrayFromListMethod = method.MakeGenericMethod(typeof(T));
        }
        
        var result = extractArrayFromListMethod.Invoke(null, new object[]{ list });
        return result as T[];
    }

    public static void AddMappingPoint(Transform convertTrans, List<Vector2> points, List<Vector3> points3D, Vector3 point)
    {
        points.Add(convertTrans.InverseTransformPoint(point));
        points3D.Add(point);
    }

    public static void InsertMappingPoint(Transform convertTrans, List<Vector2> points, List<Vector3> points3D, int index, Vector3 point)
    {
        points.Insert(index, convertTrans.InverseTransformPoint(point));
        points3D.Insert(index, point);
    }

    public static void UpdateMappingPoint(Transform convertTrans, List<Vector2> points, List<Vector3> points3D, int index, Vector3 point)
    {
        points[index] = convertTrans.InverseTransformPoint(point);
        points3D[index] = point;
    }

    public static void RemoveMappingPoint(List<Vector2> points, List<Vector3> points3D, int index)
    {
        points.RemoveAt(index);
        points3D.RemoveAt(index);
    }
}