using System;
using System.Collections.Generic;
using System.Threading;
using ChainDefense.PathFinding;
using ChainDefense.UI.ProgressBar;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace ChainDefense.Enemies
{
    public class Enemy : MonoBehaviour, IHasProgress
    {
        private const float MIN_DISTANCE_TO_WAYPOINT = 0.1f;
        public event EventHandler<IHasProgress.ProgressEventArgs> OnProgressUpdate;
        public static event EventHandler OnAnyEnemyReachedBase; //TODO: Clear static events
        //public static event EventHandler OnAnyEnemyDestroyed; //TODO: Clear static events

        public event EventHandler<int> OnEnemyTakeDamage;
        public event EventHandler OnEnemySlowedStart;
        public event EventHandler OnEnemySlowedFinish;

        [SerializeField] private EnemySO _enemySO;

        private int _currentHealth;
        private int _currentWaypointIndex;
        private bool _isDead;
        private int _currentAttackDamage;
        private float _currentSpeed;
        private float _speedMultiplier = 1f;
        private PathManager _pathManager;
        private EnemySpawner _enemySpawner;
        private CancellationTokenSource _slowCts;

        private void Awake()
        {
            _currentHealth = _enemySO.MaxHealth;
            _currentSpeed = _enemySO.BaseMoveSpeed;
            _currentWaypointIndex = 0;
            _isDead = false;
            _currentAttackDamage = _enemySO.BaseDamage;
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

            transform.position += _currentSpeed * _speedMultiplier * Time.deltaTime * moveDirection;
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

                OnAnyEnemyReachedBase?.Invoke(this, EventArgs.Empty);
            }
        }

        public async UniTaskVoid ApplySlow(float percent, float duration)
        {
            _slowCts?.Cancel();
            _slowCts?.Dispose();

            _slowCts = new CancellationTokenSource();

            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                _slowCts.Token,
                this.GetCancellationTokenOnDestroy()
            );

            _speedMultiplier = 1f - percent;
            OnEnemySlowedStart?.Invoke(this, EventArgs.Empty);
            var cancelled = await UniTask.Delay(
                TimeSpan.FromSeconds(duration),
                cancellationToken: linkedCts.Token
            ).SuppressCancellationThrow();

            linkedCts?.Dispose();

            if (!cancelled)
            {
                _speedMultiplier = 1f;
                OnEnemySlowedFinish?.Invoke(this, EventArgs.Empty);
            }
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;

            OnProgressUpdate?.Invoke(this,
                new IHasProgress.ProgressEventArgs(GetNormalizedProgress()));

            OnEnemyTakeDamage?.Invoke(this, damage);

            if (_currentHealth <= 0)
                SelfDestroy();
        }

        public void SelfDestroy()
        {
            _isDead = true;
            _enemySpawner.ReturnEnemy(this);
        }

        public int GetWaypointIndex() =>
            _currentWaypointIndex;

        public bool GetIsDead() =>
            _isDead;

        public static Enemy SpawnEnemy(EnemySO config, Vector3 position) =>
            EnemySpawner.Instance.SpawnEnemy(config, position);
        public static int GetAliveEnemyCount() =>
            EnemySpawner.Instance.GetAliveCount();
        
        public static IReadOnlyList<Enemy> GetAliveEnemies() =>
            EnemySpawner.Instance.GetAliveEnemies();
        
        public static event EventHandler<Enemy> OnEnemySpawned
        {
            add => EnemySpawner.Instance.OnEnemySpawned += value;
            remove => EnemySpawner.Instance.OnEnemySpawned -= value;
        }
        
        public static event EventHandler<Enemy> OnEnemyDestroyed
        {
            add => EnemySpawner.Instance.OnEnemyDestroyed += value;
            remove => EnemySpawner.Instance.OnEnemyDestroyed -= value;
        }
        
        public static event EventHandler<int> OnAliveCountChanged
        {
            add => EnemySpawner.Instance.OnAliveCountChanged += value;
            remove => EnemySpawner.Instance.OnAliveCountChanged -= value;
        }

        public float GetNormalizedProgress() =>
            _currentHealth / (float)_enemySO.MaxHealth;

        public int GetAttackDamage() =>
            _currentAttackDamage;
    }
}