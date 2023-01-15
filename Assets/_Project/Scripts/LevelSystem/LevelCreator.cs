using System.IO;
using System.Collections.Generic;
using System.Linq;
using GridSystem;
using Pieces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LevelSystem
{
    public class LevelCreator : MonoBehaviour
    {
        [Space] [SerializeField] private Piece meshPrefab;

        [SerializeField] [TitleGroup("Level Data", order: 0)]
        private Vector3 origin;

        [SerializeField] [PropertyOrder(-2)] [HorizontalGroup("LevelCount")]
        private int levelIndex;


        [TitleGroup("Level Data", order: 0)] [SerializeField] [MinValue(4)] [MaxValue(6)]
        private Vector2Int gridSize = new(4, 4);

        [HideInInspector] public PieceData[] pieces;

        private GridData _gridData;

        public GridController GridController { get; private set; }

        private string LevelsDirectoryPath => Application.dataPath + "/_Project/Resources/Levels";

        private void OnValidate()
        {
            CreateGrid();
        }

        private void Awake()
        {
            Destroy(this);
        }


        public void Load()
        {
            var text = Resources.Load($"Levels/Level {levelIndex}") as TextAsset;

            if (!text)
            {
                gridSize = new Vector2Int(4, 4);
                pieces = null;
                CreateGrid();
                return;
            }

            var levelData = JsonUtility.FromJson<LevelData>(text.text);
            gridSize = levelData.gridSize;
            pieces = levelData.pieces;
            origin = levelData.origin;
            CreateGrid();
        }


        public void Save()
        {
            var difficulty = GetDifficultyByPieceCount(pieces.Length);
            var levelData = new LevelData(gridSize, difficulty, pieces, origin);
            var jsonString = JsonUtility.ToJson(levelData);
            var directory = LevelsDirectoryPath + $"/Level {levelIndex}.json";
            File.WriteAllText(directory, jsonString);
        }

        private static LevelDifficulty GetDifficultyByPieceCount(int count)
        {
            return count switch
            {
                < 7 => LevelDifficulty.Easy,
                < 10 => LevelDifficulty.Medium,
                _ => LevelDifficulty.Hard
            };
        }

        private void CreateGrid()
        {
            _gridData = new GridData(gridSize, origin, 1);
            GridController = new GridController(_gridData);
            GridController.GenerateGrid();
        }

        public void CreateMeshVisualizer(Tri tri, Color color, int index)
        {
            var proceduralMesh = Instantiate(meshPrefab, transform.GetChild(index));
            var pieceData = new PieceData()
            {
                origin = Vector3.zero,
                vertices = tri.Points,
                triangles = new[] {0, 1, 2},
            };
            proceduralMesh.Initialize(pieceData, transform.position, color);
        }

        public void DestroyMeshVisualizer(int shapeIndex, int triIndex)
        {
            DestroyImmediate(transform.GetChild(shapeIndex).GetChild(triIndex).gameObject);
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
    }
}