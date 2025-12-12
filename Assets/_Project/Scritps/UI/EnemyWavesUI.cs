using ChainDefense.Waves;
using TMPro;
using UnityEngine;

namespace ChainDefense.UI
{
    public class EnemyWavesUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _wavesIndexLabel;
        [SerializeField] private TextMeshProUGUI _waveTimerLabel;

        private WaveManager _waveManager;

        private void Start()
        {
            _waveManager = WaveManager.Instance;

            _waveManager.OnWavesListUpdate += (_, _) => UpdateWavesNumber();
            _waveManager.OnEnemyWaveSpawned += (_, _) => UpdateWavesNumber();
            _waveManager.OnWaveCooldownChanged += (_, _) => UpdateTimerLabel();

            UpdateWavesNumber();

            UpdateTimerLabel();
        }

        private void UpdateTimerLabel()
        {
            _waveTimerLabel.text =
                $"Next Wave In: {Mathf.CeilToInt(_waveManager.GetDelayBeforeWaveTimer()).ToString()}s";
        }

        private void UpdateWavesNumber()
        {
            _wavesIndexLabel.text =
                $"Wave: {(_waveManager.GetWaveIndex() + 1).ToString()} / {_waveManager.GetTotalWavesCount().ToString()}";
        }
    }
}