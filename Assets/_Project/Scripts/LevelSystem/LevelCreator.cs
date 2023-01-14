using System.IO;
using System.Collections.Generic;
using System.Linq;
using GridSystem;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    [Space] [SerializeField] private ProceduralMesh meshPrefab;


    [PropertyOrder(-3)] [SerializeField] private LevelDifficulty difficulty;

    [SerializeField] private Vector3 origin;

    [SerializeField] [PropertyOrder(-2)] [HorizontalGroup("LevelCount")]
    private int levelIndex;

    [ReadOnly, SerializeField, HorizontalGroup("LevelCount")]
    private int levelCount;


    [TitleGroup("Level Data", order: 0)] [SerializeField] [MinValue(4)] [MaxValue(6)]
    private Vector2Int gridSize = new Vector2Int(4, 4);


    [TitleGroup("Level Data")] [SerializeField, ReadOnly]
    public PieceData[] pieces;

    private readonly List<LevelData> _levels = new();

    private GridData _gridData;

    public GridController GridController { get; private set; }

    private string LevelsDirectoryPath => Application.dataPath + "/_Project/Resources/Levels";

    private void OnValidate()
    {
        CreateGrid();
    }

    private void Awake()
    {
        LoadLevels();
        SetLevelIndexByLevelCount();
    }


    public void Load()
    {
        var text = Resources.Load($"Levels/Level {levelIndex}") as TextAsset;

        if (!text)
        {
            gridSize = new Vector2Int(4, 4);
            pieces = null;
            return;
        }

        var levelData = JsonUtility.FromJson<LevelData>(text.text);
        gridSize = levelData.gridSize;
        pieces = levelData.pieces;
        origin = levelData.origin;
        difficulty = levelData.levelDifficulty;
        CreateGrid();
    }


    public void Save()
    {
        var levelData = new LevelData(gridSize, difficulty, pieces, origin);
        var jsonString = JsonUtility.ToJson(levelData);
        var directory = LevelsDirectoryPath + $"/Level {levelIndex}.json";
        File.WriteAllText(directory, jsonString);
    }

    private void SetLevelIndexByLevelCount()
    {
        levelIndex = _levels.Count;
        levelCount = _levels.Count;
    }

    private void LoadLevels()
    {
        var texts = Resources.LoadAll(LevelsDirectoryPath).Select(text => (TextAsset) text);
        foreach (var text in texts)
        {
            var levelData = JsonUtility.FromJson<LevelData>(text.text);
            _levels.Add(levelData);
        }
    }

    private void CreateGrid()
    {
        _gridData = new GridData(gridSize, origin, 1);
        GridController = new GridController(_gridData);
        GridController.GenerateGrid();
    }

    private void OnDrawGizmos()
    {
        if (GridController == null)
            CreateGrid();

        var cellSize = Vector3.one;

        Gizmos.color = Color.yellow;
        for (int x = 0; x < _gridData.Size.x; x++)
        {
            for (int y = 0; y < _gridData.Size.y; y++)
            {
                var cellPosition = GridController.CalculateCellPosition(x, y, _gridData.Size.x, origin);
                Gizmos.DrawWireCube(cellPosition, cellSize);
            }
        }
    }

    public void CreateMeshVisualizer(Tri tri, Color color, int index)
    {
        var proceduralMesh = Instantiate(meshPrefab, transform.GetChild(index));
        var pieceData = new PieceData()
        {
            origin = Vector3.zero,
            vertices = tri.Points,
            triangles = new[] {0, 1, 2},
            color = color
        };
        proceduralMesh.Initialize(pieceData);
    }

    public void DestroyMeshVisualizer(int shapeIndex, int triIndex)
    {
        DestroyImmediate(transform.GetChild(shapeIndex).GetChild(triIndex).gameObject);
    }
}