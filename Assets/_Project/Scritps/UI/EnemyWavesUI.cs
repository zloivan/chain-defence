using IKhom.StateMachineSystem.Runtime;
using TMPro;
using UnityEngine;

namespace ChainDefense
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
            _waveManager.OnEnemyNumberChanged += (_, _) => UpdateLabels();
            _waveManager.OnWaveCooldownChanged += UpdateWaveCooldown;

            UpdateLabels();
        }

        private void UpdateWaveCooldown(object sender, float cooldown)
        {
            _waveTimerLabel.text = $"Next Wave In: {Mathf.CeilToInt(cooldown)}s";
        }

        private void UpdateLabels()
        {
            _enemyCounterLabel.text = $"Enemies Left: {_waveManager.GetNumberOfEnemiesAlive().ToString()}";
            _wavesIndexLabel.text = $"Wave: {_waveManager.GetWaveIndex() + 1} / {_waveManager.GetTotalWavesCount()}";
            
        }

       
    }
}