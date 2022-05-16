using UnityEngine;


namespace EditorExtensions
{
    public class SelectionInfo
    {
        public int lineIndex = -1;
        public bool mouseIsOverLine;

        public int pointIndex = -1;
        public bool mouseIsOverPoint;
        public bool pointIsSelected;
        public Vector3 positionAtStartOfDrag;
    }
}