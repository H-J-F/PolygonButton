using UnityEngine;


public static class Util
{
    public static bool PolygonOverlap(Vector2[] points, Vector2 target)
    {
        return PolygonOverlap(points, target.x, target.y);
    }

    // 可以将该方法移至你的工具类
    public static bool PolygonOverlap(Vector2[] points, float targetX, float targetY)
    {
        bool result = false;

        for (int i = 0, j = points.Length - 1; i < points.Length; i++)
        {
            bool betweenY = points[i].y > targetY != points[j].y > targetY;
            if (betweenY)
            {
                // 检查x是在i、j交点的左边还是右边
                if (targetX < (points[j].x - points[i].x) / (points[j].y - points[i].y) * (targetY - points[i].y) + points[i].x)
                {
                    result = !result;
                }
            }

            j = i;
        }

        return result;
    }

    public static bool CircleOverlap(Vector2 center, float radius, Vector2 target)
    {
        return CircleOverlap(center, radius, target.x, target.y);
    }

    public static bool CircleOverlap(Vector2 center, float radius, float targetX, float targetY)
    {
        return (double) radius * radius > (double) (targetY - center.y) * (targetY - center.y) + (double) (targetX - center.x) * (targetX - center.x);
    }
}