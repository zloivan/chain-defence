using System;
using ChainDefense.Core;
using ChainDefense.Events;
using ChainDefense.LevelManagement;
using IKhom.EventBusSystem.Runtime;
using IKhom.ServiceLocatorSystem.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace ChainDefense.UI
{
    public class GamePauseScreenUI : BaseScreenUI
    {
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _settingsButton;


        private GameplayController _gameplayController;
        private LevelManager _levelManager;

        private void Start()
        {
            _gameplayController = ServiceLocator.ForSceneOf(this).Get<GameplayController>();
            _levelManager = ServiceLocator.ForSceneOf(this).Get<LevelManager>();

            _gameplayController.OnGamePaused += GameplayController_OnGamePaused;
            _gameplayController.OnGameResumed += GameplayController_OnGameResumed;

            _resumeButton.onClick.AddListener(() => { _gameplayController.SwitchPauseState(); });
            _restartButton.onClick.AddListener(() =>
            {
                _gameplayController.SwitchPauseState();
                _levelManager.RestartLevel();
            });
            _settingsButton.onClick.AddListener(() => { Debug.Log("Load Settings"); });
            Hide();
        }

        private void OnDestroy()
        {
            _gameplayController.OnGamePaused -= GameplayController_OnGamePaused;
            _gameplayController.OnGameResumed -= GameplayController_OnGameResumed;
            _resumeButton.onClick.RemoveAllListeners();
            _restartButton.onClick.RemoveAllListeners();
            _settingsButton.onClick.RemoveAllListeners();
        }

        private void GameplayController_OnGameResumed(object o, EventArgs eventArgs) =>
            Hide();

        private void GameplayController_OnGamePaused(object o, EventArgs eventArgs) =>
            Show();
    }
}