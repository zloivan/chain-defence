using System;
using System.Collections.Generic;
using ChainDefense.Events;
using ChainDefense.MapManagement;
using ChainDefense.SavingSystem;
using ChainDefense.Waves;
using Cysharp.Threading.Tasks;
using IKhom.EventBusSystem.Runtime;
using IKhom.UtilitiesLibrary.Runtime.components;
using UnityEngine;

namespace ChainDefense.LevelManagement
{
    //TODO: this guy should hide everything and ask for scene number to load
    public class LevelManager : SingletonBehaviour<LevelManager>
    {
        [SerializeField] private List<LevelSO> _levelList;
        [SerializeField] private bool _loadFirstOnStart;

        private MapManager _mapManager;
        private LevelSO _currentLevel;
        private WaveManager _waveManager;
        private SaveManager _saveManager;

        private void Start()
        {
            _mapManager = MapManager.Instance;
            _waveManager = WaveManager.Instance;
            _saveManager = SaveManager.Instance;
            _waveManager.OnAllWavesCompleted += OnWaveManager_OnAllWavesCompleted;

            var highestLevelUnlocked = _saveManager.GetLastCompletedLevelNumber();
            if (highestLevelUnlocked == 0)
            {
                LoadLevelByIndex(0);
                return;
            }

            LoadLevelByIndex((highestLevelUnlocked + 1) % _levelList.Count);
        }

        private void OnWaveManager_OnAllWavesCompleted(object o, EventArgs eventArgs)
        {
            EventBus<LevelCompletedEvent>.Raise(
                new LevelCompletedEvent(GetCurrentLevelIndex(), GetCurrentLevelNumber()));

            //TODO: TEMP - gonna restart the scene here
            LoadLevelByIndex((_levelList.IndexOf(_currentLevel) + 1) % _levelList.Count);
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
            _waveManager.RunWaveSequence().Forget();
        }

        public int GetCurrentLevelNumber() =>
            _currentLevel.LevelNumber;

        public int GetCurrentLevelIndex() =>
            _levelList.IndexOf(_currentLevel);

        public int GetNumberOfLevels() =>
            _levelList.Count;
    }
}