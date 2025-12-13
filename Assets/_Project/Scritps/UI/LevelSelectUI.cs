using System;
using System.Collections.Generic;
using ChainDefense.SavingSystem;
using ChainDefense.UI;
using UnityEngine;
using UnityEngine.UI;

namespace ChainDefense.MainMenu
{
    public class LevelSelectUI : MonoBehaviour
    {
        [SerializeField] private List<LevelStageUI> _leveStageList;


        private SaveManager _saveManager;
        private int _highestCompletedLevelIndex;

        private void Start()
        {
            _saveManager = SaveManager.Instance;
            _highestCompletedLevelIndex = _saveManager.GetHighestCompletedLevelIndex();

            UpdateLevelStageVisuals();
        }

        private void UpdateLevelStageVisuals()
        {
            for (int i = 0; i < _leveStageList.Count; i++)
            {
                var stage = i + 1;
                var state = i < _highestCompletedLevelIndex ? LevelStageUI.StageState.Completed :
                    i > _highestCompletedLevelIndex ? LevelStageUI.StageState.Locked : LevelStageUI.StageState.Active;
                _leveStageList[i].SetStageVisual(state);
                _leveStageList[i].SetLevelNumber(stage);
                _leveStageList[i].SetTopPath(i < _leveStageList.Count - 1);
                _leveStageList[i].SetDownPath(i > 0);
            }
        }
    }
}