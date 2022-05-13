using UnityEngine;
using UnityEngine.UI;


namespace UIExtensions
{
    [AddComponentMenu("UI/Extensions/PolygonImage")]
    [RequireComponent(typeof(PolygonCollider2D))]
    public class PolygonImage : Image
    {
        private PolygonCollider2D polygon = null;

        public PolygonCollider2D Polygon
        {
            get
            {
                if (null == polygon) polygon = GetComponent<PolygonCollider2D>();
                return polygon;
            }
        }


        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPoint, eventCamera, out var pos);
            return Polygon.OverlapPoint(pos);
        }

#if UNITY_EDITOR

        protected override void Reset()
        {
            base.Reset();
            var size = rectTransform.sizeDelta;
            float w = size.x * 0.5f - 1f;
            float h = size.y * 0.5f - 1f;
            Polygon.points = new[]
            {
                new Vector2(-w,-h),
                new Vector2(w,-h),
                new Vector2(w,h),
                new Vector2(-w,h)
            };
        }

#endif
    }
}
