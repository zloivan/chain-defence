using ChainDefense.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ChainDefense.UI
{
    public class PauseButtonUI : MonoBehaviour
    {
        [SerializeField] private Button _pauseButton;
        [SerializeField] private GameObject _pausePopup;

        private void Awake()
        {
            _pauseButton.onClick.AddListener(() =>
            {
                GameplayController.Instance.SwitchPauseState();
            });
        }
    }
}