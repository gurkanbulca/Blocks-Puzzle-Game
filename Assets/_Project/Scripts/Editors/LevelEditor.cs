using System.IO;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public class LevelEditor : OdinEditorWindow
{
    [Space] [OnValueChanged("SetLevelIndexByDifficulty")] [PropertyOrder(-3)] [SerializeField]
    private LevelDifficulty difficulty;

    [SerializeField] [PropertyOrder(-2)] private int levelIndex;


    [TitleGroup("Level Data", order: 0)] [SerializeField] [MinValue(4)] [MaxValue(6)]
    private Vector2Int gridSize = new Vector2Int(4, 4);


    [TitleGroup("Level Data")] [SerializeField]
    private PieceData[] pieces;

    private Dictionary<LevelDifficulty, List<LevelData>> _levels;

    private string LevelsDirectoryPath => Application.dataPath + "/_Project/Resources/Levels";

    [MenuItem("Tools/Level Editor")]
    private static void OpenWindow()
    {
        GetWindow<LevelEditor>().Show();
    }

    private void Awake()
    {
        LoadLevels();
        SetLevelIndexByDifficulty();
    }

    [GUIColor(0, 1, 0)]
    [Button(ButtonSizes.Large)]
    [ButtonGroup("SaveAndLoad", order: -1)]
    private void Load()
    {
        var text = Resources.Load($"Levels/{difficulty.ToString()}/Level {levelIndex}") as TextAsset;

        if (!text)
        {
            gridSize = new Vector2Int(4, 4);
            pieces = null;
            return;
        }
        var levelData = JsonUtility.FromJson<LevelData>(text.text);
        gridSize = levelData.gridSize;
        pieces = levelData.pieces;
    }

    [GUIColor(1, 0, 0)]
    [Button(ButtonSizes.Large)]
    [ButtonGroup("SaveAndLoad", order: -1)]
    private void Save()
    {
        var levelData = new LevelData(gridSize, difficulty, pieces);
        var jsonString = JsonUtility.ToJson(levelData);
        var directory = LevelsDirectoryPath + $"/{levelData.levelDifficulty.ToString()}/";
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(directory + $"Level {levelIndex}.json", jsonString);
    }

    private void SetLevelIndexByDifficulty()
    {
        if (_levels.TryGetValue(difficulty, out var levels))
        {
            levelIndex = levels.Count;
            return;
        }

        levelIndex = 0;
    }

    private void LoadLevels()
    {
    }
}