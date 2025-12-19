using System;
using System.Collections.Generic;
using System.Threading;
using ChainDefense.Events;
using ChainDefense.PathFinding;
using ChainDefense.UI.ProgressBar;
using Cysharp.Threading.Tasks;
using IKhom.EventBusSystem.Runtime;
using UnityEngine;

namespace ChainDefense.Enemies
{
    public class Enemy : MonoBehaviour, IHasProgress
    {
        private const float MIN_DISTANCE_TO_WAYPOINT = 0.1f;
        public event EventHandler<IHasProgress.ProgressEventArgs> OnProgressUpdate;
        public static event EventHandler OnAnyEnemyReachedBase; //TODO: Move that to base logic. via trigger

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
        private int _maxHealth;

        public void Initialize(EnemySO enemyConfig, PathManager pathManager, EnemySpawner enemySpawner, int levelNumber)
        {
            _maxHealth = CalculateMaxHealth(enemyConfig.MaxHealth, levelNumber);
            _currentHealth = _maxHealth;
            _currentSpeed = enemyConfig.BaseMoveSpeed;
            _currentAttackDamage = enemyConfig.BaseDamage;

            _pathManager = pathManager;
            _enemySpawner = enemySpawner;

            transform.position = _pathManager.GetSpawnPosition();
            _currentWaypointIndex = 0;
            _isDead = false;
        }

        private void Update() =>
            MoveToWaypoint();

        private int CalculateMaxHealth(int enemyConfigMaxHealth, int levelNumber)
        {
            const float PER_LEVEL_HEALTH_MULTIPLIER = 0.15f;
            
            return Mathf.RoundToInt(enemyConfigMaxHealth * (1 + levelNumber * PER_LEVEL_HEALTH_MULTIPLIER));
        }

        private void MoveToWaypoint()
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
            EventBus<EnemyTakeDamageEvent>.Raise(new EnemyTakeDamageEvent());
            
            if (_currentHealth > 0) 
                return;
            
            SelfDestroy();
        }

        public void SelfDestroy()
        {
            _isDead = true;
            _enemySpawner.ReturnEnemy(this);
            EventBus<EnemyDestroyedEvent>.Raise(new EnemyDestroyedEvent());
        }

        public int GetWaypointIndex() =>
            _currentWaypointIndex;

        public bool GetIsDead() =>
            _isDead;



        public float GetNormalizedProgress() =>
            _currentHealth / (float)_maxHealth;

        public int GetAttackDamage() =>
            _currentAttackDamage;

    }
}