using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UIExtensions
{
    [AddComponentMenu("UI/Extensions/PolygonImage")]
    public class PolygonImage : Image
    {
        public float pointRadius = 0.8f;

        [SerializeField]
        private List<Vector2> points = new List<Vector2>();

        public List<Vector2> Points
        {
            get => points ??= new List<Vector2>();
            set => points = value;
        }


        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            if (points == null || points.Count <= 0)
                return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPoint, eventCamera);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out var pos);
            return Overlap(pos);
        }

        public bool Overlap(Vector2 target)
        {
            return Util.PolygonOverlap(Points, target);
        }
    }
}
