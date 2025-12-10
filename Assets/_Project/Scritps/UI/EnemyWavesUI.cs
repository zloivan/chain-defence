using IKhom.StateMachineSystem.Runtime;
using TMPro;
using UnityEngine;

namespace ChainDefense
{
    public class EnemyWavesUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _enemyCounterLabel;
        [SerializeField] private TextMeshProUGUI _wavesIndexLabel;

        private WaveManager _waveManager;

        private void Start()
        {
            _waveManager = WaveManager.Instance;
            _waveManager.OnEnemyNumberChanged += (_, _) => UpdateLabels();

            UpdateLabels();
        }

        private void UpdateLabels()
        {
            _enemyCounterLabel.text = $"Enemies Left: {_waveManager.GetNumberOfEnemiesAlive().ToString()}";
            _wavesIndexLabel.text = $"Wave: {_waveManager.GetWaveIndex() + 1} / {_waveManager.GetTotalWavesCount()}";
        }
    }
}