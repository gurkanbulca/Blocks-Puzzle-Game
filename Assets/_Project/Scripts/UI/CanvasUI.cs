using GameStateSystem;
using TMPro;
using UnityEngine;

public class CanvasUI : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text difficultyText;
    [SerializeField] private GameObject levelSuccess;

    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        GameStateController.OnCurrentGameStateChanged += HandleGameStateChange;
    }

    private void OnDestroy()
    {
        GameStateController.OnCurrentGameStateChanged -= HandleGameStateChange;
    }

    private void HandleGameStateChange(GameState state)
    {
        levelSuccess.SetActive(state == GameState.Success);
        if (state == GameState.Play)
            SetGameUI();
    }

    private void SetGameUI()
    {
        levelText.text = "Level " + (_gameManager.CurrentLevelIndex + 1);
        difficultyText.text = "Difficulty:" + _gameManager.CurrentLevelDifficulty;
    }
}