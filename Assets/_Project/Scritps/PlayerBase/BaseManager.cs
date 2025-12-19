using System;
using ChainDefense.Enemies;
using ChainDefense.Events;
using IKhom.EventBusSystem.Runtime;
using UnityEngine;

namespace ChainDefense.PlayerBase
{
    public class BaseManager : MonoBehaviour
    {
        public event EventHandler<int> OnBaseTakeDamage;
        private const int MAX_HEALTH = 20;

        private int _currentHealth;
        private EventBinding<EnemyReachedBaseEvent> _eventBinding;

        private void Awake() =>
            _currentHealth = MAX_HEALTH;

        private void Start()
        {
            _eventBinding = new EventBinding<EnemyReachedBaseEvent>(Enemy_OnAnyEnemyReachedBase);
            EventBus<EnemyReachedBaseEvent>.Register(_eventBinding);
        }

        private void OnDestroy() =>
            EventBus<EnemyReachedBaseEvent>.Deregister(_eventBinding);

        private void Enemy_OnAnyEnemyReachedBase(EnemyReachedBaseEvent eventInfo)
        {
            TakeDamage(eventInfo.Enemy.GetAttackDamage());
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            OnBaseTakeDamage?.Invoke(this, _currentHealth);
            EventBus<BaseTakeDamageEvent>.Raise(new BaseTakeDamageEvent());

            if (_currentHealth > 0)
                return;

            _currentHealth = 0;
            EventBus<BaseDestroyedEvent>.Raise(new BaseDestroyedEvent());
        }

        public int GetCurrentHealth() =>
            _currentHealth;

        public float GetNormalizedHealth() =>
            (float)_currentHealth / MAX_HEALTH;

        public int GetMaxHealth() =>
            MAX_HEALTH;
    }
}