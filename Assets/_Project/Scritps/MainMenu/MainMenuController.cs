using System;
using ChainDefense.SavingSystem;
using ChainDefense.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChainDefense.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private BaseScreenUI _levelSelectScreen;
        [SerializeField] private BaseScreenUI _homeScreen;
        [SerializeField] private BaseScreenUI _settingsScreenScreen;

        private SaveManager _saveManager;
        
        private void Awake()
        {
            _homeScreen.Show();
            _levelSelectScreen.Hide();
            _settingsScreenScreen.Hide();
        }

        private void Start()
        {
            _saveManager = SaveManager.Instance;
        }

        public void OpenSettingsScreen()
        {
            Debug.Log("Load Settings");

            _settingsScreenScreen.Show();
            _levelSelectScreen.Hide();
            _homeScreen.Hide();
        }

        public void OpenLevelSelectScreen()
        {
            Debug.Log("Load Level Selector");

            _levelSelectScreen.Show();
            _homeScreen.Hide();
            _settingsScreenScreen.Hide();
        }

        public void OpenHomeScreen()
        {
            Debug.Log("Load Home Screen");

            _homeScreen.Show();
            _levelSelectScreen.Hide();
            _settingsScreenScreen.Hide();
        }

        public void Loadgame()
        {
            SceneManager.LoadScene("Game");
        }
    }
}