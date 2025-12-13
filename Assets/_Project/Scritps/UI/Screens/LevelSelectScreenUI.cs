using ChainDefense.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace ChainDefense.UI
{
    public class LevelSelectUI : BaseScreenUI
    {
        [SerializeField] private MainMenuController _mainMenuController;
        [SerializeField] private Button _homeScreen;
        [SerializeField] private Button _settingsScreen;
        [SerializeField] private Button _startButton;
        

        private void Awake()
        {
            _homeScreen.onClick.AddListener(() => _mainMenuController.OpenHomeScreen());
            _settingsScreen.onClick.AddListener(() => _mainMenuController.OpenSettingsScreen());
            _startButton.onClick.AddListener(() => _mainMenuController.OpenLevelSelectScreen());
        }
    }
}