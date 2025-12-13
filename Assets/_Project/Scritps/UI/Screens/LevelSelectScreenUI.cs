using System;
using ChainDefense.MainMenu;
using ChainDefense.SavingSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChainDefense.UI
{
    public class LevelSelectScreenUI : BaseScreenUI
    {
        [SerializeField] private MainMenuController _mainMenuController;
        [SerializeField] private Button _homeScreen;
        [SerializeField] private Button _settingsScreen;
        [SerializeField] private Button _startButton;

        private TextMeshProUGUI _buttonText;
        private SaveManager _saveManager;

        private void Awake()
        {
            _buttonText = _startButton.GetComponentInChildren<TextMeshProUGUI>();
            _homeScreen.onClick.AddListener(() => _mainMenuController.OpenHomeScreen());
            _settingsScreen.onClick.AddListener(() => _mainMenuController.OpenSettingsScreen());
            _startButton.onClick.AddListener(() => _mainMenuController.Loadgame());
        }

        private void Start()
        {
            _saveManager = SaveManager.Instance;
            
            _buttonText.text = $"Level {_saveManager.GetLastCompletedLevelNumber() + 1}";
        }
    }
}