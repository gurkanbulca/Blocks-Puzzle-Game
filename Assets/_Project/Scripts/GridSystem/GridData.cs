using UnityEngine;

namespace GridSystem
{
    public class GridData
    {
        public Vector2Int Size;
        public Vector3 Origin;
        public readonly float Spacing;


        public GridData(Vector2Int size, Vector3 origin, float spacing)
        {
            Size = size;
            Origin = origin;
            Spacing = spacing;
        }
    }
}