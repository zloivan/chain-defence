using System;
using ChainDefense.Enemies;
using IKhom.UtilitiesLibrary.Runtime.components;

namespace ChainDefense.PlayerBase
{
    public class BaseManager : SingletonBehaviour<BaseManager>
    {
        public event EventHandler OnGameOver;
        public event EventHandler<int> OnBaseTakeDamage;
        private const int MAX_HEALTH = 20;

        private int _currentHealth;

        protected override void Awake()
        {
            base.Awake();

            _currentHealth = MAX_HEALTH;
        }

        private void Start()
        {
            Enemy.OnAnyEnemyReachedBase += Enemy_OnAnyEnemyReachedBase;
        }

        private void Enemy_OnAnyEnemyReachedBase(object sender, EventArgs e)
        {
            if (sender is Enemy enemy)
                TakeDamage(enemy.GetAttackDamage());
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            OnBaseTakeDamage?.Invoke(this, _currentHealth);

            if (_currentHealth > 0) 
                return;
            
            _currentHealth = 0;
            OnGameOver?.Invoke(this, EventArgs.Empty);
        }

        public int GetCurrentHealth() =>
            _currentHealth;
        
        public float GetNormalizedHealth() =>
            (float)_currentHealth / MAX_HEALTH;
        
        public int GetMaxHealth() =>
            MAX_HEALTH;
    }
}