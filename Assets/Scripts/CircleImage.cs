using UnityEngine;
using UnityEngine.UI;


namespace UIExtensions
{
    [AddComponentMenu("UI/Extensions/CircleImage")]
    public class CircleImage : Image
    {
        public bool useCollider = false;

        [SerializeField]
        private float radius;

        public float Radius
        {
            get => radius;
            set
            {
                var size = rectTransform.sizeDelta;
                float max = size.x > size.y ? size.x / 2f : size.y / 2f;
                radius = Mathf.Clamp(value, 0f, max);
            }
        }


        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            if (!useCollider)
                return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPoint, eventCamera);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out var pos);
            return Overlap(pos);
        }

        public bool Overlap(Vector2 target)
        {
            return Util.CircleOverlap(Vector2.zero, radius, target);
        }
    }
}