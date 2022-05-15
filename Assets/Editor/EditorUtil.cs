using System.Collections;
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
}