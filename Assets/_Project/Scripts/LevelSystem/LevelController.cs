using UnityEngine;

namespace LevelSystem
{
    public static class LevelController
    {
        public static int CurrentLevelIndex { get; private set; } = -1;

        public static LevelData GetRandomLevelData()
        {
            var levels = Resources.LoadAll("Levels");
            CurrentLevelIndex = Random.Range(0, levels.Length);
            var levelText = levels[CurrentLevelIndex] as TextAsset;
            if (!levelText)
                return null;

            return JsonUtility.FromJson<LevelData>(levelText.text);
        }
    }
}