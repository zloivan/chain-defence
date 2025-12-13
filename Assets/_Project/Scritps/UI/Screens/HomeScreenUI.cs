using System;
using ChainDefense.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace ChainDefense.UI
{
    public class HomeScreenUI : BaseScreenUI
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private MainMenuController _mainMenuController;

        private void Awake()
        {
            _settingsButton.onClick.AddListener(() => _mainMenuController.OpenSettingsScreen());
            _startButton.onClick.AddListener(() => _mainMenuController.OpenLevelSelectScreen());
        }
    }
}