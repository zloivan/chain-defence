using ChainDefense.Enemies;
using ChainDefense.Waves;
using TMPro;
using UnityEngine;

namespace ChainDefense.UI
{
    public class EnemyWavesUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _enemyCounterLabel;
        [SerializeField] private TextMeshProUGUI _wavesIndexLabel;
        [SerializeField] private TextMeshProUGUI _waveTimerLabel;

        private WaveManager _waveManager;

        private void Start()
        {
            _waveManager = WaveManager.Instance;
            Enemy.OnAliveCountChanged += (_, _) => UpdateUI();
            _waveManager.OnEnemyWaveSpawned += (_, _) => UpdateUI();
            _waveManager.OnWaveCooldownChanged += UpdateWaveCooldown;

            UpdateUI();
        }

        private void UpdateWaveCooldown(object sender, float cooldown) =>
            _waveTimerLabel.text = $"Next Wave In: {Mathf.CeilToInt(cooldown).ToString()}s";

        private void UpdateUI()
        {
            _enemyCounterLabel.text = $"Enemies Left: {Enemy.GetAliveEnemyCount().ToString()}";
            _wavesIndexLabel.text =
                $"Wave: {(_waveManager.GetWaveIndex() + 1).ToString()} / {_waveManager.GetTotalWavesCount().ToString()}";
        }
    }
}