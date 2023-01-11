using System;
using GridSystem;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Vector3 origin;
    [SerializeField] private Vector2Int gridSize = new(2, 2);

    [SerializeField] private float spacing = 1f;


    private GridController _gridController;

    private GridData _gridData;
    
    private void OnValidate()
    {
        CreateGrid();
    }

    private void Awake()
    {
    }

    private void Start()
    {
        var levelData = LevelController.GetRandomLevelData(LevelDifficulty.Easy);
        CreateGrid();
        gridSize = levelData.gridSize;
        Debug.Log(levelData);
    }

    

    private void CreateGrid()
    {
        _gridData = new GridData(gridSize, origin, spacing);
        _gridController = new GridController(_gridData);
    }

    private void OnDrawGizmos()
    {
        if (_gridController == null)
            CreateGrid();

        var cellSize = Vector3.one * spacing;

        Gizmos.color = Color.yellow;
        for (int x = 0; x < _gridData.Size.x; x++)
        {
            for (int y = 0; y < _gridData.Size.y; y++)
            {
                var cellPosition = _gridController.CalculateCellPosition(x, y, _gridData.Size.x, origin);
                Gizmos.DrawWireCube(cellPosition, cellSize);
            }
        }
    }
}