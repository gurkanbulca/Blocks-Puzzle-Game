using UnityEngine;

namespace LevelSystem
{
    public static class LevelController
    {
        public static int CurrentLevelIndex { get; private set; }

        public static LevelData GetLevelData(int index)
        {
            var levels = Resources.LoadAll("Levels");
            CurrentLevelIndex = index;
            var levelText = levels[CurrentLevelIndex] as TextAsset;
            return !levelText ? null : JsonUtility.FromJson<LevelData>(levelText.text);
        }
    }
}