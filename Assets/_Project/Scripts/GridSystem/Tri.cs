using System.Linq;
using UnityEngine;

namespace GridSystem
{
    public class Tri
    {
        public bool IsFull;
        
        public readonly Vector3 Origin;
        public readonly Vector3[] Points;
        
        public Tri(Vector3[] points, Vector3 cellOrigin)
        {
            Points = points.Select(point => point + cellOrigin).ToArray();
            Origin = Vector3.zero;
            foreach (var point in Points)
            {
                Origin += point;
            }

            Origin /= Points.Length;
        }
    }
}