using System;
using System.Collections.Generic;
using ChainDefense.LevelManagement;
using ChainDefense.PathFinding;
using IKhom.UtilitiesLibrary.Runtime.components;
using UnityEngine;

namespace ChainDefense.Enemies
{
    public class EnemySpawner : SingletonBehaviour<EnemySpawner>
    {
        public event EventHandler<Enemy> OnEnemySpawned;
        public event EventHandler<Enemy> OnEnemyDestroyed;
        public event EventHandler<int> OnAliveCountChanged;

        private readonly List<Enemy> _aliveEnemies = new();
        private PathManager _pathManager;
        private LevelManager _levelManager;

        private void Start()
        {
            _pathManager = PathManager.Instance;
            _levelManager = LevelManager.Instance;
        }

        public Enemy SpawnEnemy(EnemySO enemySO, Vector3 position)
        {
            var enemyGameObject = Instantiate(enemySO.EnemyPrefab, position, Quaternion.identity, transform);
            var enemy = enemyGameObject.GetComponent<Enemy>();
            enemy.Initialize(enemySO, _pathManager, this, _levelManager.GetCurrentLevelNumber());

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