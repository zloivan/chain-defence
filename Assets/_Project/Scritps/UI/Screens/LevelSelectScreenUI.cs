using System;
using ChainDefense.MainMenu;
using ChainDefense.SavingSystem;
using IKhom.ServiceLocatorSystem.Runtime;
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
            _saveManager = ServiceLocator.Global.Get<SaveManager>();
            
            var nextLevelIndex = _saveManager.GetLastCompletedLevelNumber() + 1;
            var nextLevelNumber = nextLevelIndex + 1;
            
            _buttonText.text = $"Level {nextLevelNumber}";
        }

        private void OnDestroy()
        {
            _homeScreen.onClick.RemoveAllListeners();
            _settingsScreen.onClick.RemoveAllListeners();
            _startButton.onClick.RemoveAllListeners();
        }
    }
}