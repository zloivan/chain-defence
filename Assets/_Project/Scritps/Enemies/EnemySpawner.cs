using System;
using System.Collections.Generic;
using ChainDefense.LevelManagement;
using ChainDefense.PathFinding;
using IKhom.ServiceLocatorSystem.Runtime;
using UnityEngine;

namespace ChainDefense.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        private readonly List<Enemy> _aliveEnemies = new();
        private PathManager _pathManager;
        private LevelManager _levelManager;

        private void Start()
        {
            _pathManager = ServiceLocator.ForSceneOf(this).Get<PathManager>();
            _levelManager = ServiceLocator.ForSceneOf(this).Get<LevelManager>();
        }

        public Enemy SpawnEnemy(EnemySO enemySO, Vector3 position)
        {
            var enemyGameObject = Instantiate(enemySO.EnemyPrefab, position, Quaternion.identity, transform);
            var enemy = enemyGameObject.GetComponent<Enemy>();
            enemy.Initialize(enemySO, _pathManager, this, _levelManager.GetCurrentLevelNumber());

            _aliveEnemies.Add(enemy);
            return enemy;
        }

        public void ReturnEnemy(Enemy enemy)
        {
            _aliveEnemies.Remove(enemy);
            Destroy(enemy.gameObject);
        }

        public int GetAliveCount() =>
            _aliveEnemies.Count;

        public IReadOnlyList<Enemy> GetAliveEnemies() =>
            _aliveEnemies.AsReadOnly();

        public void ClearAllEnemies()
        {
            foreach (var enemy in _aliveEnemies)
            {
                Destroy(enemy.gameObject);
            }

            _aliveEnemies.Clear();
        }
    }
}