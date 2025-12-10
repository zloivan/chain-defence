using System;
using ChainDefense.PathFinding;
using Unity.VisualScripting;
using UnityEngine;

namespace ChainDefense.Enemies
{
    public class Enemy : MonoBehaviour
    {
        private const float MIN_DISTANCE_TO_WAYPOINT = 0.1f;

        public event EventHandler OnEnemyReachedBase;
        public static event EventHandler OnEnemyDestroyed;

        [SerializeField] private EnemySO _enemySO;

        private int _currentHealth;
        private int _currentWaypointIndex;

        private PathManager _pathManager;
        private EnemySpawner _enemySpawner;
        

        private void Awake()
        {
            _currentHealth = _enemySO.MaxHealth;
            _currentWaypointIndex = 0;
        }

        private void Start()
        {
            _pathManager = PathManager.Instance;
            _enemySpawner = EnemySpawner.Instance;
            transform.position = _pathManager.GetSpawnPosition();
        }

        private void Update()
        {
            var targetPosition = _pathManager.GetWaypointPosition(_currentWaypointIndex);
            if (Vector3.Distance(targetPosition, transform.position) < MIN_DISTANCE_TO_WAYPOINT)
            {
                _currentWaypointIndex++;
                if (_currentWaypointIndex >= _pathManager.GetWaypointCount())
                {
                    // Reached the end
                    SelfDestroy();

                    OnEnemyReachedBase?.Invoke(this, EventArgs.Empty);
                    return;
                }

                targetPosition = _pathManager.GetWaypointPosition(_currentWaypointIndex);
            }

            var moveDirection = (targetPosition - transform.position).normalized;
            transform.position += _enemySO.MoveSpeed * Time.deltaTime * moveDirection;
        }

        public void SelfDestroy()
        {
            //Destroy(gameObject);
            _enemySpawner.ReturnEnemy(this);
            
            OnEnemyDestroyed?.Invoke(this, EventArgs.Empty);
        }

        public static GameObject SpawnEnemy(EnemySO config, Vector3 position) =>
            EnemySpawner.Instance.SpawnEnemy(config, position);
    }
}