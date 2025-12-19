using System;
using System.Collections.Generic;
using ChainDefense.SavingSystem;
using ChainDefense.UI;
using Cysharp.Threading.Tasks;
using IKhom.ServiceLocatorSystem.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace ChainDefense.MainMenu
{
    public class LevelSelectUI : MonoBehaviour
    {
        [SerializeField] private List<LevelStageUI> _leveStageList;
        [SerializeField] private ScrollRect _scrollRect;

        private SaveManager _saveManager;
        private int _highestCompletedLevelIndex;

        private void Start()
        {
            _saveManager = ServiceLocator.Global.Get<SaveManager>();
            
            _highestCompletedLevelIndex = _saveManager.GetHighestCompletedLevelIndex();

            UpdateLevelStageVisuals();
            
            ScrollToActiveLevelAsync().Forget();
        }

        private void UpdateLevelStageVisuals()
        {
            for (var i = 0; i < _leveStageList.Count; i++)
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
        
        private async UniTaskVoid ScrollToActiveLevelAsync()
        {
            // Wait for one frame to ensure layout is calculated
            await UniTask.Yield();
            
            ScrollToActiveLevel();
        }

        private void ScrollToActiveLevel()
        {
            if (_leveStageList.Count == 0 || _highestCompletedLevelIndex < 0 || _highestCompletedLevelIndex >= _leveStageList.Count)
                return;

            var content = _scrollRect.content;
            var viewport = _scrollRect.viewport;
            var targetItem = _leveStageList[_highestCompletedLevelIndex].GetComponent<RectTransform>();

            if (content == null || viewport == null || targetItem == null)
                return;

            Canvas.ForceUpdateCanvases();

            // Calculate the position of the target item relative to the content
            var contentHeight = content.rect.height;
            var viewportHeight = viewport.rect.height;
            
            // Get the target item's position in the content
            var targetPosition = -targetItem.localPosition.y;
            
            // Center the target item in the viewport
            var targetOffset = targetPosition - viewportHeight / 2f + targetItem.rect.height / 2f;
            
            // Calculate normalized position (0 = bottom, 1 = top for vertical scroll)
            var maxScroll = contentHeight - viewportHeight;
            var normalizedPosition = maxScroll > 0 ? Mathf.Clamp01(targetOffset / maxScroll) : 0f;
            
            // Set the vertical normalized position (inverted: 1 = top, 0 = bottom)
            _scrollRect.verticalNormalizedPosition = 1f - normalizedPosition;
        }
    }
}