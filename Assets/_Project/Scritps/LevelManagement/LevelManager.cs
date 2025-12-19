using System;
using System.Collections.Generic;
using ChainDefense.Events;
using ChainDefense.MapManagement;
using ChainDefense.SavingSystem;
using ChainDefense.Waves;
using Cysharp.Threading.Tasks;
using IKhom.EventBusSystem.Runtime;
using IKhom.ServiceLocatorSystem.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChainDefense.LevelManagement
{
    //TODO: this guy should hide everything and ask for scene number to load
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private List<LevelSO> _levelList;

        private MapManager _mapManager;
        private LevelSO _currentLevel;
        private WaveManager _waveManager;
        private SaveManager _saveManager;

        private bool _levelCompleted = false;

        private void Start()
        {
            _mapManager = MapManager.Instance;
            _waveManager = ServiceLocator.ForSceneOf(this).Get<WaveManager>();
            _saveManager = SaveManager.Instance;
            _waveManager.OnAllWavesCompleted += OnWaveManager_OnAllWavesCompleted;

            var requiredLevelToPlay = _saveManager.GetRequiredLevelToPlay();
            var highestLevelUnlocked = _saveManager.GetLastCompletedLevelNumber();
            
            if (requiredLevelToPlay != -1 && requiredLevelToPlay < GetNumberOfLevels())
            {
                LoadLevelByIndex(requiredLevelToPlay);
                return;
            }

            LoadLevelByIndex((highestLevelUnlocked + 1) % _levelList.Count);
        }

        private void RestartScene()
        {
            SceneManager.LoadScene("Game");
        }

        private void OnWaveManager_OnAllWavesCompleted(object o, EventArgs eventArgs)
        {
            EventBus<LevelCompletedEvent>.Raise(
                new LevelCompletedEvent(GetCurrentLevelIndex(), GetCurrentLevelNumber()));

            _levelCompleted = true;
        }

        public void RestartLevel()
        {
            if (_levelCompleted)
            {
                _levelCompleted = false;
                RequireLevelToPlay(GetCurrentLevelIndex());
            }
            else
            {
                RestartScene();
            }
        }

        public void NextLevel()
        {
            if (!_levelCompleted)
            {
                RequireLevelToPlay((GetCurrentLevelIndex() + 1) % _levelList.Count);
            }
            else
            {
                RestartScene();
            }
        }

        private void RequireLevelToPlay(int levelIndex)
        {
            _saveManager.SetRequireLevelToPlay(levelIndex);
            RestartScene();
        }

        private void LoadLevelByIndex(int levelIndex)
        {
            _currentLevel = _levelList[levelIndex];

            Debug.Assert(_currentLevel, "Current Level is not assigned", this);

            LoadLevel();
        }

        private void LoadLevel()
        {
            _mapManager.LoadMap(_currentLevel.Map);
            _waveManager.SetupWavesList(_currentLevel.Waves);
            _waveManager.RunWaveSequence(_currentLevel.FirstWaveDelay).Forget();
        }

        public int GetCurrentLevelNumber() =>
            _currentLevel.LevelNumber;

        public int GetCurrentLevelIndex() =>
            _levelList.IndexOf(_currentLevel);

        public int GetNumberOfLevels() =>
            _levelList.Count;
    }
}