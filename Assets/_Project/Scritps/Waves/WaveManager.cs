using System;
using System.Collections.Generic;
using System.Threading;
using ChainDefense.Enemies;
using ChainDefense.PathFinding;
using Cysharp.Threading.Tasks;
using IKhom.UtilitiesLibrary.Runtime.components;
using UnityEngine;

namespace ChainDefense.Waves
{
    public class WaveManager : SingletonBehaviour<WaveManager>
    {
        public class WaveEventArgs : EventArgs
        {
            public readonly int WaveIndex;
            public IReadOnlyList<Enemy> SpawnedEnemies;

            public WaveEventArgs(int waveIndex, IReadOnlyList<Enemy> spawnedEnemies)
            {
                WaveIndex = waveIndex;
                SpawnedEnemies = spawnedEnemies;
            }
        }

        public event EventHandler<WaveEventArgs> OnEnemyWaveSpawned;
        public event EventHandler OnAllWavesCompleted;
        public event EventHandler<float> OnWaveCooldownChanged;
        public event EventHandler OnWavesListUpdate;

        [SerializeField] private List<WaveSO> _wavesList;
        [SerializeField] private bool _useMockWaves;

        private int _currentWaveIndex;
        private PathManager _pathManager;
        private float _delayBeforeWaveTimer;
        private CancellationTokenSource _waveSequenceCts;

        private void Start()
        {
            _pathManager = PathManager.Instance;
            Enemy.OnDestroyed += Enemy_OnDestroyed;

            if (_useMockWaves)
            {
                RunWaveSequence().Forget();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _waveSequenceCts?.Cancel();
            _waveSequenceCts?.Dispose();
        }

        public void SetupWavesList(List<WaveSO> wavesList)
        {
            _wavesList = wavesList;
            OnWavesListUpdate?.Invoke(this, EventArgs.Empty);
            _currentWaveIndex = 0;
        }

        private void Enemy_OnDestroyed(object sender, Enemy enemy)
        {
            if (Enemy.GetAliveEnemyCount() > 0)
                return;

            CompleteCurrentWave();
        }

        public async UniTask RunWaveSequence(float initialDelay = 0f)
        {
            _waveSequenceCts?.Cancel();
            _waveSequenceCts?.Dispose();

            _waveSequenceCts = new CancellationTokenSource();
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                _waveSequenceCts.Token,
                this.GetCancellationTokenOnDestroy()
            );

            var cancelled = await UniTask.Delay(
                TimeSpan.FromSeconds(initialDelay),
                cancellationToken: linkedCts.Token
            ).SuppressCancellationThrow();

            if (cancelled)
            {
                linkedCts?.Dispose();
                return;
            }

            while (_currentWaveIndex < _wavesList.Count)
            {
                var currentWave = _wavesList[_currentWaveIndex];

                cancelled = await RunWaveCooldown(currentWave.DelayBeforeStarting, linkedCts.Token);
                if (cancelled)
                {
                    linkedCts?.Dispose();
                    return;
                }

                cancelled = await SpawnWave(currentWave, linkedCts.Token);
                if (cancelled)
                {
                    linkedCts?.Dispose();
                    return;
                }

                if (currentWave.EnemyTypes == null || currentWave.EnemyTypes.Length == 0)
                {
                    continue;
                }

                linkedCts?.Dispose();
                return;
            }

            linkedCts?.Dispose();
        }

        private async UniTask<bool> RunWaveCooldown(float cooldownDuration, CancellationToken cancellationToken)
        {
            _delayBeforeWaveTimer = cooldownDuration;
            OnWaveCooldownChanged?.Invoke(this, _delayBeforeWaveTimer);

            while (_delayBeforeWaveTimer >= 0)
            {
                var cancelled = await UniTask.Yield(cancellationToken).SuppressCancellationThrow();
                if (cancelled) return true;

                _delayBeforeWaveTimer -= Time.deltaTime;
                OnWaveCooldownChanged?.Invoke(this, _delayBeforeWaveTimer);
            }

            return false;
        }

        private async UniTask<bool> SpawnWave(WaveSO targetWave, CancellationToken cancellationToken)
        {
            // Case when wave has no enemies
            if (targetWave.EnemyTypes == null || targetWave.EnemyTypes.Length == 0)
            {
                CompleteCurrentWave();
                return false;
            }

            foreach (var enemyGroup in targetWave.EnemyTypes)
            {
                for (var i = 0; i < enemyGroup.EnemyCount; i++)
                {
                    Enemy.SpawnEnemy(enemyGroup.EnemyType, _pathManager.GetSpawnPosition());

                    if (i < enemyGroup.EnemyCount - 1)
                    {
                        var cancelled = await UniTask.Delay(
                            TimeSpan.FromSeconds(enemyGroup.TimeBetweenSpawns),
                            cancellationToken: cancellationToken
                        ).SuppressCancellationThrow();

                        if (cancelled) return true;
                    }
                }

                OnEnemyWaveSpawned?.Invoke(this, new WaveEventArgs(_currentWaveIndex, Enemy.GetAliveEnemies()));
            }

            return false;
        }

        private void CompleteCurrentWave()
        {
            _currentWaveIndex++;

            if (_currentWaveIndex >= _wavesList.Count)
            {
                OnAllWavesCompleted?.Invoke(this, EventArgs.Empty);
                return;
            }

            RunWaveSequence().Forget();
        }

        public int GetWaveIndex() =>
            _currentWaveIndex;

        public int GetTotalWavesCount() =>
            _wavesList.Count;

        public float GetDelayBeforeWaveTimer() =>
            _delayBeforeWaveTimer;
    }
}