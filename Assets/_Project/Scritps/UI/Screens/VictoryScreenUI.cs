using System;
using ChainDefense.Events;
using ChainDefense.LevelManagement;
using IKhom.EventBusSystem.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace ChainDefense.UI
{
    public class VictoryScreenUI : BaseScreenUI
    {
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _homeScreen;
        [SerializeField] private Button _nextLevelButton;
        
        private LevelManager _levelManager;
        private EventBinding<LevelCompletedEvent> _eventBinding;

        private void Awake()
        {
            _eventBinding = new EventBinding<LevelCompletedEvent>(OnLevelCompleted);
            EventBus<LevelCompletedEvent>.Register(_eventBinding);
        }

        private void Start()
        {
            _levelManager = LevelManager.Instance;
            _restartButton.onClick.AddListener(() => { _levelManager.RestartLevel(); });
            _homeScreen.onClick.AddListener(() => Debug.Log("Open Home Screen")); //TODO: Placeholder
            _nextLevelButton.onClick.AddListener(() => _levelManager.NextLevel());

            Hide();
        }

        private void OnDestroy() =>
            EventBus<LevelCompletedEvent>.Deregister(_eventBinding);

        private void OnLevelCompleted(LevelCompletedEvent eventData) =>
            Show();
    }
}