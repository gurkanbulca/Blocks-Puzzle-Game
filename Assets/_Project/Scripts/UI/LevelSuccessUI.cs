using GameCore;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LevelSuccessUI : MonoBehaviour
    {
        private Button _button;
        private GameManager _gameManager;

        private void Awake()
        {
            _button = GetComponentInChildren<Button>();
            _button.onClick.AddListener(HandleButtonClick);
            _gameManager = FindObjectOfType<GameManager>();
        }

        private void HandleButtonClick()
        {
            _gameManager.CompleteLevel();
        }
    }
}