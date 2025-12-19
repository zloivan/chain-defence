using ChainDefense.Core;
using IKhom.ServiceLocatorSystem.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace ChainDefense.UI
{
    public class PauseButtonUI : MonoBehaviour
    {
        [SerializeField] private Button _pauseButton;
        [SerializeField] private GameObject _pausePopup;

        private GameplayController _gameplayController;
        
        private void Start()
        {
            _gameplayController = ServiceLocator.ForSceneOf(this).Get<GameplayController>();
            
            _pauseButton.onClick.AddListener(() =>
            {
                _gameplayController.SwitchPauseState();
            });
        }
        
        private void OnDestroy() =>
            _pauseButton.onClick.RemoveAllListeners();
    }
}