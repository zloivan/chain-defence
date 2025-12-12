using System;
using System.Collections.Generic;
using ChainDefense.MapManagement;
using ChainDefense.Waves;
using Cysharp.Threading.Tasks;
using IKhom.UtilitiesLibrary.Runtime.components;
using UnityEngine;

namespace ChainDefense.LevelManagement
{
    //TODO: this guy should hide everything and ask for scene number to load
    public class LevelManager : SingletonBehaviour<LevelManager>
    {
        public event EventHandler OnLevelComplete;
        [SerializeField] private List<LevelSO> _levelList;
        [SerializeField] private bool _loadFirstOnStart;

        private MapManager _mapManager;
        private LevelSO _currentLevel;
        private WaveManager _waveManager;

        private void Start()
        {
            _mapManager = MapManager.Instance;
            _waveManager = WaveManager.Instance;

            _waveManager.OnAllWavesCompleted += OnLevelComplete;

            //TODO: HERE WE FIND LEVEL NUMBER FROM OUTSIDE
            if (_loadFirstOnStart)
            {
                LoadLevelByIndex(0);
            }
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
    }
}