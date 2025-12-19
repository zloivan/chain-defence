using ChainDefense.Events;
using ChainDefense.LevelManagement;
using IKhom.EventBusSystem.Runtime;
using IKhom.ServiceLocatorSystem.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace ChainDefense.UI
{
    public class DefeatScreenUI : BaseScreenUI
    {
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _homeScreenButton;


        private EventBinding<GameOverEvent> _eventBinding;
        private LevelManager _levelManager;

        private void Awake()
        {
            _eventBinding = new EventBinding<GameOverEvent>(OnGameOverEvent);
            EventBus<GameOverEvent>.Register(_eventBinding);
        }

        private void Start()
        {
            _levelManager = ServiceLocator.ForSceneOf(this).Get<LevelManager>();

            _restartButton.onClick.AddListener(() => { _levelManager.RestartLevel(); });
            _homeScreenButton.onClick.AddListener(() => Debug.Log("Open Home Screen")); //TODO: Placeholder

            Hide();
        }

        private void OnDestroy() =>
            EventBus<GameOverEvent>.Deregister(_eventBinding);

        private void OnGameOverEvent(GameOverEvent obj) =>
            Show();
    }
}