using System;
using System.Linq;
using UnityEngine;

namespace GridSystem
{
    public class GridController
    {
        private readonly GridData _gridData;


        public Cell[,] grid { get; private set; }


        public GridController(GridData gridData)
        {
            _gridData = gridData;
        }


        public void GenerateGrid()
        {
            grid = new Cell[_gridData.Size.x, _gridData.Size.y];
            for (int x = 0; x < _gridData.Size.x; x++)
            {
                for (int y = 0; y < _gridData.Size.y; y++)
                {
                    var cell = new Cell(CalculateCellPosition(x, y, _gridData.Size.x, _gridData.Origin),
                        new Vector2Int(x, y));
                    grid[x, y] = cell;
                }
            }
        }


        public Vector3 CalculateCellPosition(int x, int y, int columnCount, Vector3 origin)
        {
            if (x < 0 || y < 0 || columnCount < 0)
                throw new ArgumentOutOfRangeException(nameof(x), "Value can not be negative!");

            return new Vector3(
                (origin.x - _gridData.Spacing * columnCount / 2f + _gridData.Spacing / 2f) + _gridData.Spacing * x,
                origin.y - _gridData.Spacing * y, origin.z);
        }


        public Cell[] GetColumn(int index)
        {
            return Enumerable.Range(0, grid.GetLength(1))
                .Select(x => grid[index, x])
                .ToArray();
        }


        public Cell GetCell(int x, int y)
        {
            return grid[x, y];
        }

        public Cell GetClosestCell(Vector3 point)
        {
            Cell closestCell = null;
            float closestDistance = Mathf.Infinity;
            foreach (var cell in grid)
            {
                var distance = Vector3.Distance(point, cell.Position);
                if (distance < closestDistance)
                {
                    closestCell = cell;
                    closestDistance = distance;
                }
            }

            return closestCell;
        }

        public bool IsPointInGridBorders(Vector3 point)
        {
            return point.x >= grid[0, 0].Position.x - .5f
                   && point.x <= grid[grid.GetLength(0) - 1, 0].Position.x + .5f
                   && point.y >= grid[0, grid.GetLength(1) - 1].Position.y - .5f
                   && point.y <= grid[0, 0].Position.y + .5f;
        }
    }
}