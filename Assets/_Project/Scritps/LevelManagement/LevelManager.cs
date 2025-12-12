using System;
using ChainDefense.MapManagement;
using ChainDefense.Waves;
using IKhom.UtilitiesLibrary.Runtime.components;

namespace ChainDefense.LevelManagement
{
    public class LevelManager : SingletonBehaviour<LevelManager>
    {
        public event EventHandler OnLevelComplete; 
        
        private MapManager _mapManager;
        private LevelSO _currentLevel;
        private WaveManager _waveManager;
        
        private void Start()
        {
            _mapManager = MapManager.Instance;
            _waveManager = WaveManager.Instance;
        }

        private void LoadLevel(int levelIndex)
        {
            _mapManager.LoadMap(_currentLevel.Map);
            
        }
    }
}