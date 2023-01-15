using UnityEngine;

namespace Pieces
{
    public class PieceTriangle
    {
        public Vector3 Origin;

        public PieceTriangle(Vector3[] points)
        {
            Origin = Vector3.zero;
            foreach (var point in points)
            {
                Origin += point;
            }

            Origin /= points.Length;
        }
    }
}