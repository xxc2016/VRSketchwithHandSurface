using UnityEngine;

namespace VRPainting
{
    public class LineInfo
    {
        public Color color;

        public float width;

        public LineInfo(Color c,float w)
        {
            color = c;
            width = w;
        }

        public LineInfo() {
            color = Color.black;
            width = 0.01f;
        }

    }
}