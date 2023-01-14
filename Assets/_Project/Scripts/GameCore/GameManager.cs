using System;
using GridSystem;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ProceduralMesh proceduralMesh;


    private GridController _gridController;
    private GridData _gridData;
    private LevelData _levelData;
    private PieceSpawner _pieceSpawner;
    private GridDrawer _gridDrawer;

    private void Awake()
    {
        _pieceSpawner = FindObjectOfType<PieceSpawner>();
        _gridDrawer = FindObjectOfType<GridDrawer>();
    }

    private void Start()
    {
        _levelData = LevelController.GetRandomLevelData(LevelDifficulty.Easy);
        CreateGrid(_levelData);
        foreach (var pieceData in _levelData.pieces)
        {
            _pieceSpawner.SpawnPiece(pieceData);
        }

        _gridDrawer.Initialize(_gridController.grid);
    }


    private void CreateGrid(LevelData levelData)
    {
        _gridData = new GridData(levelData.gridSize, levelData.origin, 1);
        _gridController = new GridController(_gridData);
        _gridController.GenerateGrid();
    }
}