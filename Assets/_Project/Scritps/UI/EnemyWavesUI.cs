using System;
using ChainDefense.Events;
using ChainDefense.Waves;
using DG.Tweening;
using IKhom.EventBusSystem.Runtime;
using IKhom.ServiceLocatorSystem.Runtime;
using TMPro;
using UnityEngine;

namespace ChainDefense.UI
{
    public class EnemyWavesUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _wavesIndexLabel;
        [SerializeField] private TextMeshProUGUI _waveTimerLabel;

        private WaveManager _waveManager;
        private EventBinding<WaveSpawnedEvent> _eventBinding;
        private void Start()
        {
            _waveManager = ServiceLocator.ForSceneOf(this).Get<WaveManager>();
            _eventBinding = new EventBinding<WaveSpawnedEvent>(UpdateWavesNumber);
            EventBus<WaveSpawnedEvent>.Register(_eventBinding);

            _waveManager.OnWavesListUpdate += (_, _) => UpdateWavesNumber();
            _waveManager.OnWaveCooldownChanged += (_, _) => UpdateTimerLabel();

            UpdateWavesNumber();

            UpdateTimerLabel();
            _waveTimerLabel.gameObject.SetActive(false);
        }

        private void OnDestroy() =>
            EventBus<WaveSpawnedEvent>.Deregister(_eventBinding);

        private int _previousNumberOfSeconds = -1;
        private bool _isTimerVisible = false;

        private void UpdateTimerLabel()
        {
            var timerValue = _waveManager.GetDelayBeforeWaveTimer();
            var currentNumberOfSeconds = Mathf.CeilToInt(timerValue);

            // Timer should be visible when it's actively counting (greater than 0)
            var shouldBeVisible = timerValue > 0;

            if (shouldBeVisible != _isTimerVisible)
            {
                _isTimerVisible = shouldBeVisible;
                _waveTimerLabel.gameObject.SetActive(shouldBeVisible);

                if (shouldBeVisible) 
                    _waveTimerLabel.rectTransform.DOShakeAnchorPos(0.3f, strength: 100f, vibrato: 30);
            }

            if (_isTimerVisible && currentNumberOfSeconds != _previousNumberOfSeconds)
            {
                _previousNumberOfSeconds = currentNumberOfSeconds;
                _waveTimerLabel.text = currentNumberOfSeconds.ToString();

                // Small shake on each second
                _waveTimerLabel.rectTransform.DOShakeAnchorPos(0.2f, strength: 50f, vibrato: 30);
            }
            else if (!_isTimerVisible)
            {
                // Reset previous seconds when timer is hidden
                _previousNumberOfSeconds = -1;
            }
        }

        private void UpdateWavesNumber()
        {
            _wavesIndexLabel.text =
                $"{(_waveManager.GetWaveIndex() + 1).ToString()} / {_waveManager.GetTotalWavesCount().ToString()}";
        }
    }
}