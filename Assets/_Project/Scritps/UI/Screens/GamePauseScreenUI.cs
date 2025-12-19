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

            _gameplayController.OnGamePaused += (_, _) => { Show(); };
            _gameplayController.OnGameResumed += (_, _) => { Hide(); };

            _resumeButton.onClick.AddListener(() => { _gameplayController.SwitchPauseState(); });
            _restartButton.onClick.AddListener(() =>
            {
                _gameplayController.SwitchPauseState();
                _levelManager.RestartLevel();//TODO: ADD ORCHISTRETOR
            });
            _settingsButton.onClick.AddListener(() => { Debug.Log("Load Settings"); });
            Hide();
        }
    }
}