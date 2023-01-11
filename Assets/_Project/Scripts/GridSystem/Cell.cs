using UnityEngine;

namespace GridSystem
{
    public class Cell
    {
        #region Public Fields

        public readonly Vector3 Position;
        public readonly Vector2Int GridCoordinate;

        #endregion

        #region Constructor

        public Cell(Vector3 position, Vector2Int coordinate)
        {
            Position = position;
            GridCoordinate = coordinate;
        }

        #endregion
    }
}