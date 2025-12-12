using ChainDefense.Waves;
using TMPro;
using UnityEngine;

namespace ChainDefense.UI
{
    public class EnemyWavesUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _wavesIndexLabel;
        [SerializeField] private TextMeshProUGUI _waveTimerLabel;

        private void Start()
        {
            WaveManager.Instance.OnEnemyWaveSpawned += (_, waveInfo) =>
            {
                _wavesIndexLabel.text =
                    $"Wave: {(waveInfo.WaveIndex + 1).ToString()} / {WaveManager.Instance.GetTotalWavesCount().ToString()}";
            };
            WaveManager.Instance.OnWaveCooldownChanged += (_, waveDelayTimer) =>
            {
                _waveTimerLabel.text = $"Next Wave In: {Mathf.CeilToInt(waveDelayTimer).ToString()}s";
            };

            _wavesIndexLabel.text =
                $"Wave: {(WaveManager.Instance.GetWaveIndex() + 1).ToString()} / {WaveManager.Instance.GetTotalWavesCount().ToString()}";

            _waveTimerLabel.text =
                $"Next Wave In: {Mathf.CeilToInt(WaveManager.Instance.GetDelayBeforeWaveTimer()).ToString()}s";
        }
    }
}