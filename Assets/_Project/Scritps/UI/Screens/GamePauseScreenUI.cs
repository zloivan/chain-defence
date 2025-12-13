using ChainDefense.Core;
using ChainDefense.Events;
using ChainDefense.LevelManagement;
using IKhom.EventBusSystem.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace ChainDefense.UI
{
    public class GamePauseScreenUI : MonoBehaviour
    {
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _settingsButton;

        private void Awake()
        {
            GameplayController.Instance.OnGamePaused += (_, _) => { Show(); };
            GameplayController.Instance.OnGameResumed += (_, _) => { Hide(); };

            _resumeButton.onClick.AddListener(() =>
            {
                GameplayController.Instance.SwitchPauseState();
            });
            _restartButton.onClick.AddListener(() =>
            {
                GameplayController.Instance.SwitchPauseState();
                LevelManager.Instance.RestartLevel();
            });
            _settingsButton.onClick.AddListener(() => { Debug.Log("Load Settings"); });
            Hide();
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}