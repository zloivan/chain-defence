using System;
using ChainDefense.PathFinding;
using UnityEngine;

namespace ChainDefense.Enemies
{
    public class Enemy : MonoBehaviour
    {
        private const float MIN_DISTANCE_TO_WAYPOINT = 0.1f;
        
        public event EventHandler OnEnemyReachedBase;
        
        [SerializeField] private EnemySO _enemySO;

        private int _currentHealth;
        private int _currentWaypointIndex;

        PathManager _pathManager;


        private void Awake()
        {
            _currentHealth = _enemySO.MaxHealth;
            _currentWaypointIndex = 0;
        }

        private void Start()
        {
            _pathManager = PathManager.Instance;

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
                    Destroy(gameObject);
                    OnEnemyReachedBase?.Invoke(this, EventArgs.Empty);
                    return;
                }

                targetPosition = _pathManager.GetWaypointPosition(_currentWaypointIndex);
            }

            var moveDirection = (targetPosition - transform.position).normalized;
            transform.position += _enemySO.MoveSpeed * Time.deltaTime * moveDirection;
        }
    }
}