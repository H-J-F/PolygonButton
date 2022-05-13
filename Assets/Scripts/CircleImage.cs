using UnityEngine;
using UnityEngine.UI;


namespace UIExtensions
{
    [AddComponentMenu("UI/Extensions/CircleImage")]
    [RequireComponent(typeof(CircleCollider2D))]
    public class CircleImage : Image
    {
        private CircleCollider2D circle = null;

        public CircleCollider2D Circle
        {
            get
            {
                if (null == circle) circle = GetComponent<CircleCollider2D>();
                return circle;
            }
        }


        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPoint, eventCamera, out var pos);
            return Circle.OverlapPoint(pos);
        }

#if UNITY_EDITOR

        protected override void Reset()
        {
            base.Reset();
            var size = rectTransform.sizeDelta;
            Circle.radius = size.y / 2f;
        }

#endif
    }
}