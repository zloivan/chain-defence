using System;
using ChainDefense.SavingSystem;
using ChainDefense.UI;
using IKhom.ServiceLocatorSystem.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChainDefense.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private BaseScreenUI _levelSelectScreen;
        [SerializeField] private BaseScreenUI _homeScreen;
        [SerializeField] private BaseScreenUI _settingsScreenScreen;

        private void Awake()
        {
            _homeScreen.Show();
            _levelSelectScreen.Hide();
            _settingsScreenScreen.Hide();
        }

        public void OpenSettingsScreen()
        {

            _settingsScreenScreen.Show();
            _levelSelectScreen.Hide();
            _homeScreen.Hide();
        }

        public void OpenLevelSelectScreen()
        {

            _levelSelectScreen.Show();
            _homeScreen.Hide();
            _settingsScreenScreen.Hide();
        }

        public void OpenHomeScreen()
        {

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