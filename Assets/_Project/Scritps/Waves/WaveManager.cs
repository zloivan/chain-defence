using System;
using System.Collections.Generic;
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

        private void Start()
        {
            _pathManager = PathManager.Instance;
            Enemy.OnDestroyed += OnDestroyed;

            if (_useMockWaves)
            {
                RunWaveSequence().Forget();
            }
        }
        
        public void SetupWavesList(List<WaveSO> wavesList)
        {
            _wavesList = wavesList;
            OnWavesListUpdate?.Invoke(this, EventArgs.Empty);
            _currentWaveIndex = 0;
        }

        private void OnDestroyed(object sender, Enemy enemy)
        {
            if (Enemy.GetAliveEnemyCount() > 0)
                return;

            CompleteCurrentWave();
        }

        public async UniTask RunWaveSequence(float initialDelay = 0f)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(initialDelay));

            while (_currentWaveIndex < _wavesList.Count)
            {
                var currentWave = _wavesList[_currentWaveIndex];

                await RunWaveCooldown(currentWave.DelayBeforeStarting);

                await SpawnWave(currentWave);

                if (currentWave.EnemyTypes == null || currentWave.EnemyTypes.Length == 0)
                {
                    continue;
                }

                return;
            }
        }

        private async UniTask RunWaveCooldown(float cooldownDuration)
        {
            _delayBeforeWaveTimer = cooldownDuration;
            OnWaveCooldownChanged?.Invoke(this, _delayBeforeWaveTimer);

            while (_delayBeforeWaveTimer >= 0)
            {
                await UniTask.Yield();
                _delayBeforeWaveTimer -= Time.deltaTime;
                OnWaveCooldownChanged?.Invoke(this, _delayBeforeWaveTimer);
            }
        }

        private async UniTask SpawnWave(WaveSO targetWave)
        {
            // Case when wave has no enemies
            if (targetWave.EnemyTypes == null || targetWave.EnemyTypes.Length == 0)
            {
                CompleteCurrentWave();
                return;
            }

            foreach (var enemyGroup in targetWave.EnemyTypes)
            {
                for (var i = 0; i < enemyGroup.EnemyCount; i++)
                {
                    Enemy.SpawnEnemy(enemyGroup.EnemyType, _pathManager.GetSpawnPosition());

                    if (i < enemyGroup.EnemyCount - 1)
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(enemyGroup.TimeBetweenSpawns));
                    }
                }

                OnEnemyWaveSpawned?.Invoke(this, new WaveEventArgs(_currentWaveIndex, Enemy.GetAliveEnemies()));
            }
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