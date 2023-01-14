using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static LevelData GetRandomLevelData(LevelDifficulty difficulty)
    {
        var levels = Resources.LoadAll("Levels");
        var levelText = levels[Random.Range(0, levels.Length)] as TextAsset;
        if (!levelText)
            return null;

        return JsonUtility.FromJson<LevelData>(levelText.text);
    }
}