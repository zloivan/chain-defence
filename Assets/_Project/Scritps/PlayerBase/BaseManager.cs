using System;
using ChainDefense.Enemies;
using ChainDefense.Events;
using IKhom.EventBusSystem.Runtime;
using UnityEngine;

namespace ChainDefense.PlayerBase
{
    public class BaseManager : MonoBehaviour
    {
        public event EventHandler OnGameOver;
        public event EventHandler<int> OnBaseTakeDamage;
        private const int MAX_HEALTH = 20;

        private int _currentHealth;

        private void Awake() =>
            _currentHealth = MAX_HEALTH;

        private void Start() =>
            Enemy.OnAnyEnemyReachedBase += Enemy_OnAnyEnemyReachedBase;

        private void Enemy_OnAnyEnemyReachedBase(object sender, EventArgs e)
        {
            if (sender is Enemy enemy)
                TakeDamage(enemy.GetAttackDamage());
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            OnBaseTakeDamage?.Invoke(this, _currentHealth);
            EventBus<BaseTakeDamageEvent>.Raise(new BaseTakeDamageEvent());
            
            if (_currentHealth > 0) 
                return;
            
            _currentHealth = 0;
            OnGameOver?.Invoke(this, EventArgs.Empty);
            EventBus<GameOverEvent>.Raise(new GameOverEvent()); 
        }

        public int GetCurrentHealth() =>
            _currentHealth;
        
        public float GetNormalizedHealth() =>
            (float)_currentHealth / MAX_HEALTH;
        
        public int GetMaxHealth() =>
            MAX_HEALTH;
    }
}