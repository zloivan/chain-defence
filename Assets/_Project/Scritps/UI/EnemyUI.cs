using ChainDefense.Enemies;
using TMPro;
using UnityEngine;

namespace ChainDefense.UI
{
    public class EnemyUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _enemyCounterLabel;

        private void Start()
        {
            Enemy.OnAliveCountChanged += (_, _) => { UpdateUI(); };
            UpdateUI();
        }

        private void UpdateUI() =>
            _enemyCounterLabel.text = $"Enemies Left: {Enemy.GetAliveEnemyCount().ToString()}";
    }
}