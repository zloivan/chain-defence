using System;
using ChainDefense.PathFinding;
using ChainDefense.UI.ProgressBar;
using Unity.VisualScripting;
using UnityEngine;

namespace ChainDefense.Enemies
{
    public class Enemy : MonoBehaviour, IHasProgress
    {
        private const float MIN_DISTANCE_TO_WAYPOINT = 0.1f;
        public event EventHandler<IHasProgress.ProgressEventArgs> OnProgressUpdate;
        public event EventHandler OnEnemyReachedBase;
        public static event EventHandler OnEnemyDestroyed;

        [SerializeField] private EnemySO _enemySO;

        private int _currentHealth;
        private int _currentWaypointIndex;
        private bool _isDead;

        private PathManager _pathManager;
        private EnemySpawner _enemySpawner;


        private void Awake()
        {
            _currentHealth = _enemySO.MaxHealth;
            _currentWaypointIndex = 0;
            _isDead = false;
        }

        private void Start() //TODO: with pool need reorganize initialization
        {
            _pathManager = PathManager.Instance;
            _enemySpawner = EnemySpawner.Instance;
            transform.position = _pathManager.GetSpawnPosition();
        }

        private void Update()
        {
            Navigate();
        }

        private void Navigate()
        {
            var targetPosition = _pathManager.GetWaypointPosition(_currentWaypointIndex);
            var distanceBeforeMoving = Vector3.Distance(targetPosition, transform.position);
            var moveDirection = (targetPosition - transform.position).normalized;

            transform.position += _enemySO.MoveSpeed * Time.deltaTime * moveDirection;
            var distanceAfterMoving = Vector3.Distance(targetPosition, transform.position);

            // Check if we overshot the waypoint (distance increased instead of decreased)
            if (distanceBeforeMoving >= distanceAfterMoving 
                && distanceAfterMoving >= MIN_DISTANCE_TO_WAYPOINT)
                return;

            // Snap to waypoint position
            transform.position = targetPosition;
            _currentWaypointIndex++;
            if (_currentWaypointIndex >= _pathManager.GetWaypointCount())
            {
                //TODO: latter attack the base
                SelfDestroy();

                OnEnemyReachedBase?.Invoke(this, EventArgs.Empty);
            }
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            
            OnProgressUpdate?.Invoke(this,
                new IHasProgress.ProgressEventArgs(GetNormalizedProgress()));
            
            if (_currentHealth <= 0)
                SelfDestroy();
        }

        public void SelfDestroy()
        {
            //Destroy(gameObject);
            _enemySpawner.ReturnEnemy(this);
            _isDead = true;
            OnEnemyDestroyed?.Invoke(this, EventArgs.Empty);
        }

        public int GetWaypointIndex() =>
            _currentWaypointIndex;

        public bool GetIsDead() =>
            _isDead;

        public static GameObject SpawnEnemy(EnemySO config, Vector3 position) =>
            EnemySpawner.Instance.SpawnEnemy(config, position);

        
        public float GetNormalizedProgress() =>
            _currentHealth/(float)_enemySO.MaxHealth;
    }
}