using UnityEngine;

namespace GridSystem
{
    public class Cell
    {
        #region Public Fields

        public readonly Vector3 Position;
        public readonly Vector2Int GridCoordinate;
        public Tri[] Tris;

        #endregion

        #region Constructor

        public Cell(Vector3 position, Vector2Int coordinate)
        {
            Position = position;
            GridCoordinate = coordinate;
            GenerateTris();
        }

        private void GenerateTris()
        {
            Tris = new Tri[]
            {
                new(new[] {new Vector3(0, 0, 0), new Vector3(0, .5f, 0), new Vector3(.5f, .5f, 0)}, Position),
                new(new[] {new Vector3(0, 0, 0), new Vector3(.5f, .5f, 0), new Vector3(.5f, 0, 0)}, Position),
                new(new[] {new Vector3(0, 0, 0), new Vector3(.5f, 0, 0), new Vector3(.5f, -.5f, 0)}, Position),
                new(new[] {new Vector3(0, 0, 0), new Vector3(.5f, -.5f, 0), new Vector3(0, -.5f, 0)}, Position),
                new(new[] {new Vector3(0, 0, 0), new Vector3(0, -.5f, 0), new Vector3(-.5f, -.5f, 0)}, Position),
                new(new[] {new Vector3(0, 0, 0), new Vector3(-.5f, -.5f, 0), new Vector3(-.5f, 0, 0)}, Position),
                new(new[] {new Vector3(0, 0, 0), new Vector3(-.5f, 0, 0), new Vector3(-.5f, .5f, 0)}, Position),
                new(new[] {new Vector3(0, 0, 0), new Vector3(-.5f, .5f, 0), new Vector3(0, .5f, 0)}, Position),
            };
        }

        public Tri GetClosestTri(Vector3 point)
        {
            Tri closestTri = null;
            float closestDistance = Mathf.Infinity;
            foreach (var tri in Tris)
            {
                var distance = Vector3.Distance(point, tri.Origin);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTri = tri;
                }
            }

            return closestTri;
        }

        #endregion
    }
}