using UnityEngine;
using UnityEngine.UI;


namespace UIExtensions
{
    [AddComponentMenu("UI/Extensions/PolygonRawImage")]
    [RequireComponent(typeof(PolygonCollider2D))]
    public class PolygonRawImage : RawImage, ICanvasRaycastFilter
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


        public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPoint, eventCamera, out var pos);
            return Polygon.OverlapPoint(pos);
        }

#if UNITY_EDITOR

        protected override void Reset()
        {
            base.Reset();
            var size = rectTransform.sizeDelta;
            float w = (size.x * 0.5f);
            float h = (size.y * 0.5f);
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