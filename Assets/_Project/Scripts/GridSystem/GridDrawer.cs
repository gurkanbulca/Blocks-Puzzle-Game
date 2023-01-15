using UnityEngine;
using Utils;

namespace GridSystem
{
    [RequireComponent(typeof(LineRenderer))]
    public class GridDrawer : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        private Cell[,] _grid;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        public void Initialize(Cell[,] grid)
        {
            _grid = grid;
            DrawGrid(grid);
        }

        private void DrawGrid(Cell[,] grid)
        {
            var minPoint = grid[0, 0].Position;
            minPoint.x -= .5f;
            minPoint.y += .5f;

            var maxPoint = grid[grid.GetLength(0) - 1, grid.GetLength(1) - 1].Position;
            maxPoint.x += .5f;
            maxPoint.y -= .5f;

            _lineRenderer.positionCount = 5;
            _lineRenderer.SetPosition(0, minPoint);
            _lineRenderer.SetPosition(1, new Vector3(maxPoint.x, minPoint.y));
            _lineRenderer.SetPosition(2, maxPoint);
            _lineRenderer.SetPosition(3, new Vector3(minPoint.x, maxPoint.y));
            _lineRenderer.SetPosition(4, minPoint);

            var index = 5;
            for (int i = 1; i < grid.GetLength(0); i++)
            {
                var topPoint = minPoint;
                topPoint.x += i;
                var bottomPoint = topPoint.WithY(maxPoint.y);
                _lineRenderer.positionCount += 2;
                _lineRenderer.SetPosition(index++, i % 2 == 1 ? topPoint : bottomPoint);
                _lineRenderer.SetPosition(index++, i % 2 == 1 ? bottomPoint : topPoint);
            }

            _lineRenderer.positionCount += 2;
            var point = _lineRenderer.GetPosition(index - 1);
            point = point.WithX(minPoint.x);
            _lineRenderer.SetPosition(index++, point);
            point = point.WithY(minPoint.y);
            _lineRenderer.SetPosition(index++, point);
            for (int i = 1; i < grid.GetLength(1); i++)
            {
                _lineRenderer.positionCount += 2;
                var leftPoint = minPoint;
                leftPoint.y -= i;
                var rightPoint = leftPoint.WithX(maxPoint.x);
                _lineRenderer.SetPosition(index++, i % 2 == 1 ? leftPoint : rightPoint);
                _lineRenderer.SetPosition(index++, i % 2 == 1 ? rightPoint : leftPoint);
            }
        }
    }
}