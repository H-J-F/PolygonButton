using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


namespace UIExtensions
{
    [RequireComponent(typeof(CanvasRenderer))]
    [AddComponentMenu("UI/Extensions/UI Polygon")]
    [CanEditMultipleObjects]
    public class UIPolygon : MaskableGraphic, ICanvasRaycastFilter
    {
        [SerializeField]
        Texture m_Texture;

        public bool fill = true;
        public bool useIrregularRaycast = false;
        public float thickness = 5;

        [Range(3, 360)]
        public int sides = 3;

        [Range(0, 360)]
        public float rotation = 0;

        [Range(0, 1)]
        public float[] VerticesDistances = new float[3];

        private float size = 0;
        private UIVertex[] vbo = new UIVertex[4];

        private List<Vector2[]> posList = null;
        private List<Vector2[]> PosList
        {
            get
            {
                if (posList != null) return posList;

                SetPosList(sides + 1);
                return posList;
            }
        }

        private List<Vector2[]> uvList = null;
        private List<Vector2[]> UvList
        {
            get
            {
                if (uvList != null) return uvList;
                
                SetUvList(sides + 1);
                return uvList;
            }
        }

        [SerializeField]
        private Vector2[] points;
        public Vector2[] Points
        {
            get
            {
                if (points == null || points.Length != sides)
                {
                    points = new Vector2[sides];
                }

                return points;
            }
        }

        public override Texture mainTexture => m_Texture == null ? s_WhiteTexture : m_Texture;

        public Texture texture
        {
            get
            {
                return m_Texture;
            }
            set
            {
                if (m_Texture == value) return;
                m_Texture = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        public void DrawPolygon(int _sides)
        {
            sides = _sides;
            VerticesDistances = new float[_sides + 1];
            for (int i = 0; i < _sides; i++) VerticesDistances[i] = 1; ;
            rotation = 0;
        }

        public void DrawPolygon(int _sides, float[] _VerticesDistances)
        {
            sides = _sides;
            VerticesDistances = _VerticesDistances;
            rotation = 0;
        }

        public void DrawPolygon(int _sides, float[] _VerticesDistances, float _rotation)
        {
            sides = _sides;
            VerticesDistances = _VerticesDistances;
            rotation = _rotation;
        }

        private void GetSize()
        {
            var rect = rectTransform.rect;
            size = rect.width > rect.height ? rect.height : rect.width;
            thickness = Mathf.Clamp(thickness, 0, size / 2);
        }

        protected void SetVbo(Vector2[] vertices, Vector2[] uvs)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                var vert = UIVertex.simpleVert;
                vert.color = color;
                vert.position = vertices[i];
                vert.uv0 = uvs[i];
                vbo[i] = vert;
            }
        }

        protected void SetPosList(int count)
        {
            if (posList == null) posList = new List<Vector2[]>();
            else posList.Clear();

            for (int i = 0; i < count; i++)
            {
                posList.Add(new Vector2[4]);
            }
        }

        protected void SetUvList(int count)
        {
            if (uvList == null) uvList = new List<Vector2[]>();
            else uvList.Clear();

            for (int i = 0; i < count; i++)
            {
                uvList.Add(new Vector2[4]);
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Vector2 prevX = Vector2.zero;
            Vector2 prevY = Vector2.zero;

            UvList[0][0] = new Vector2(0, 0);
            UvList[0][1] = new Vector2(0, 1);
            UvList[0][2] = new Vector2(1, 1);
            UvList[0][3] = new Vector2(1, 0);

            float degrees = 360f / sides;
            int vertices = sides + 1;
            if (VerticesDistances.Length != vertices)
            {
                VerticesDistances = new float[vertices];
                SetPosList(vertices);
                SetUvList(vertices);
                for (int i = 0; i < vertices - 1; i++) VerticesDistances[i] = 1;
            }

            GetSize();
            Vector2 pivot = rectTransform.pivot;

            // last vertex is also the first!（第一个节点也是最后一个节点）
            VerticesDistances[vertices - 1] = VerticesDistances[0];
            
            for (int i = 0; i < vertices; i++)
            {
                float outer = -pivot.x * size * VerticesDistances[i];
                float inner = -pivot.x * size * VerticesDistances[i] + thickness;
                float rad = Mathf.Deg2Rad * (i * degrees + rotation);
                float c = Mathf.Cos(rad);
                float s = Mathf.Sin(rad);

                UvList[i][0] = new Vector2(0, 1);
                UvList[i][1] = new Vector2(1, 1);
                UvList[i][2] = new Vector2(1, 0);
                UvList[i][3] = new Vector2(0, 0);

                PosList[i][0] = prevX;
                PosList[i][1] = new Vector2(outer * c, outer * s);

                if (fill)
                {
                    PosList[i][2] = Vector2.zero;
                    PosList[i][3] = Vector2.zero;
                }
                else
                {
                    PosList[i][2] = new Vector2(inner * c, inner * s);
                    PosList[i][3] = prevY;
                }
                prevX = PosList[i][1];
                prevY = PosList[i][2];
                Points[i % sides] = PosList[i][1];
                SetVbo(PosList[i], UvList[i]);
                vh.AddUIVertexQuad(vbo);
            }
        }

        public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            if (!useIrregularRaycast)
                return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPoint, eventCamera);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out var point);
            return Overlap(point);
        }

        public bool Overlap(Vector2 target)
        {
            return Util.PolygonOverlap(Points, target);
        }
    }
}