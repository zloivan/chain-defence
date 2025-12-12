using System;
using System.Collections.Generic;
using IKhom.UtilitiesLibrary.Runtime.components;
using UnityEngine;

namespace ChainDefense.Enemies
{
    public class EnemySpawner : SingletonBehaviour<EnemySpawner>
    {
        public event EventHandler<Enemy> OnEnemySpawned;
        public event EventHandler<Enemy> OnEnemyDestroyed;
        public event EventHandler<int> OnAliveCountChanged;
        
        [SerializeField] private Transform _parent;
        
        private readonly List<Enemy> _aliveEnemies = new();
        
        protected override void Awake()
        {
            if (_parent == null)
                _parent = transform;
        }

        public Enemy SpawnEnemy(EnemySO enemySO, Vector3 position)
        {
            var enemyGameObject = Instantiate(enemySO.EnemyPrefab, position, Quaternion.identity, _parent);
            var enemy = enemyGameObject.GetComponent<Enemy>();
            _aliveEnemies.Add(enemy);
            
            OnEnemySpawned?.Invoke(this, enemy);
            return enemy;
        }

        public void ReturnEnemy(Enemy enemy)
        {
            _aliveEnemies.Remove(enemy);
            OnEnemyDestroyed?.Invoke(this, enemy);
            OnAliveCountChanged?.Invoke(this, GetAliveCount());
            Destroy(enemy.gameObject);
        }
        
        public int GetAliveCount() =>
            _aliveEnemies.Count;
        
        public IReadOnlyList<Enemy> GetAliveEnemies() =>
            _aliveEnemies.AsReadOnly();
        
    }
}