using System;
using ChainDefense.Events;
using IKhom.EventBusSystem.Runtime;
using IKhom.UtilitiesLibrary.Runtime.components;
using UnityEngine;
using UnityEngine.Serialization;

namespace ChainDefense.SavingSystem
{
    public class SaveManager : SingletonBehaviour<SaveManager>
    {
        [Serializable]
        private class SaveData
        {
            public int HighestCompletedLevelIndex;
            public int LastCompletedLevelIndex;
            public bool[] CompletedLevelsArray;
        }

        private const string SAVE_DATA_PLAYER_PREFS_KEY = "SaveData";
        private SaveData _currentSave;

        private void Start()
        {
            EventBus<LevelCompletedEvent>.Register(
                new EventBinding<LevelCompletedEvent>(LevelManager_OnAnyLevelComplete));

            LoadGame();
        }

        private void LevelManager_OnAnyLevelComplete(LevelCompletedEvent eventData) =>
            SetCompletedLevel(eventData.CompletedLevelIndex);

        private void LoadGame()
        {
            if (!PlayerPrefs.HasKey(SAVE_DATA_PLAYER_PREFS_KEY))
            {
                _currentSave = GenerateSaveData();
                return;
            }

            var json = PlayerPrefs.GetString(SAVE_DATA_PLAYER_PREFS_KEY);

            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning($"Save data is empty for key {SAVE_DATA_PLAYER_PREFS_KEY}", this);
                _currentSave = GenerateSaveData();
                return;
            }

            _currentSave = JsonUtility.FromJson<SaveData>(json);
        }

        private SaveData GenerateSaveData()
        {
            var saveData = new SaveData
            {
                HighestCompletedLevelIndex = 0,
                LastCompletedLevelIndex = 0,
                CompletedLevelsArray = new bool[100],
            };

            return saveData;
        }

        private void SaveGame()
        {
            var json = JsonUtility.ToJson(_currentSave);
            PlayerPrefs.SetString(SAVE_DATA_PLAYER_PREFS_KEY, json);
        }

        private void SetCompletedLevel(int levelIndex)
        {
            if (_currentSave.CompletedLevelsArray.Length <= levelIndex)
            {
                Array.Resize(ref _currentSave.CompletedLevelsArray, levelIndex + 1);
            }
            
            _currentSave.CompletedLevelsArray[levelIndex] = true;
            _currentSave.LastCompletedLevelIndex = levelIndex;

            if (levelIndex > _currentSave.HighestCompletedLevelIndex)
            {
                _currentSave.HighestCompletedLevelIndex = levelIndex;
            }

            SaveGame();
        }

        public int GetHighestCompletedLevelIndex() =>
            _currentSave.HighestCompletedLevelIndex;

        public int GetLastCompletedLevelNumber() =>
            _currentSave.LastCompletedLevelIndex;

        public bool[] GetCompletedLevelsArray() =>
            _currentSave.CompletedLevelsArray;
    }
}