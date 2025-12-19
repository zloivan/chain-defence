using ChainDefense.Enemies;
using ChainDefense.Events;
using IKhom.EventBusSystem.Runtime;
using IKhom.ServiceLocatorSystem.Runtime;
using TMPro;
using UnityEngine;

namespace ChainDefense.UI
{
    public class EnemyUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _enemyCounterLabel;

        private EventBinding<EnemyAliveCountChangedEvent> _eventBusBinding;
        private EnemySpawner _enemySpawner;
        
        private void Start()
        {
            _enemySpawner = ServiceLocator.ForSceneOf(this).Get<EnemySpawner>();
            
            _eventBusBinding = new EventBinding<EnemyAliveCountChangedEvent>(UpdateUI);
            EventBus<EnemyAliveCountChangedEvent>.Register(_eventBusBinding);

            UpdateUI();
        }
        
        private void OnDestroy() =>
            EventBus<EnemyAliveCountChangedEvent>.Deregister(_eventBusBinding);

        private void UpdateUI() =>
            _enemyCounterLabel.text = $"Enemies Left: {_enemySpawner.GetAliveCount().ToString()}";
    }
}