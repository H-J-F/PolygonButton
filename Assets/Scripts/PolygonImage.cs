using UnityEngine;
using UnityEngine.UI;


namespace UIExtensions
{
    [AddComponentMenu("UI/Extensions/PolygonImage")]
    public class PolygonImage : Image
    {
        [SerializeField]
        private Vector2[] points = new Vector2[4];

        public Vector2[] Points
        {
            get
            {
                if (points == null || points.Length < 3)
                {
                    var temp = points;
                    points = new Vector2[3];

                    if (temp != null)
                    {
                        for (var i = 0; i < temp.Length; i++)
                        {
                            points[i] = temp[i];
                        }
                    }
                }

                return points;
            }

            set
            {
                if (value == null || value.Length < 3) return;
                points = value;
            }
        }


        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out var pos);
            return Overlap(pos);
        }

        public bool Overlap(Vector2 target)
        {
            return Util.PolygonOverlap(points, target);
        }

#if UNITY_EDITOR

        protected override void Reset()
        {
            base.Reset();
            var rect = rectTransform;
            var size = rect.sizeDelta;
            float w = size.x * 0.5f;
            float h = size.y * 0.5f;
            points = new[]
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
