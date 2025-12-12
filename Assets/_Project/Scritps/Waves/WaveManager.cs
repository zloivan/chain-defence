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
        public event EventHandler<int> OnWaveCompleted;
        public event EventHandler OnEnemyWaveSpawned;
        public event EventHandler<int> OnEnemyNumberChanged;
        public event EventHandler OnAllWavesCompleted;
        public event EventHandler<float> OnWaveCooldownChanged;


        [SerializeField] private List<WaveSO> _wavesList;

        private int _currentWaveIndex;
        private PathManager _pathManager;
        private float _cooldownTimer;

        private void Start()
        {
            _pathManager = PathManager.Instance;

            Enemy.OnEnemyDestroyed += AnyEnemyOnAnyEnemyDestroyed;

            ProcessWaves(CancellationToken.None).Forget();
        }

        private void AnyEnemyOnAnyEnemyDestroyed(object sender, Enemy enemy)
        {
            if (Enemy.GetAliveEnemyCount() > 0)
                return;

            _currentWaveIndex++;
            OnWaveCompleted?.Invoke(this, _currentWaveIndex);

            if (_currentWaveIndex >= _wavesList.Count)
            {
                OnAllWavesCompleted?.Invoke(this, EventArgs.Empty);
                return;
            }

            ProcessWaves(CancellationToken.None).Forget();
        }

        //BUG: Sometimes waves more then max and enemies less then on screen
        private async UniTask ProcessWaves(CancellationToken cancellationToken)
        {
            var currentWave = _wavesList[_currentWaveIndex];
            _cooldownTimer = currentWave.DelayBeforeStarting;
            OnWaveCooldownChanged?.Invoke(this, _cooldownTimer);

            while (_cooldownTimer >= 0)
            {
                await UniTask.Yield();
                _cooldownTimer -= Time.deltaTime;
                OnWaveCooldownChanged?.Invoke(this, _cooldownTimer);
            }

            StartWave(cancellationToken, currentWave).Forget();
        }

        private async UniTask StartWave(CancellationToken cancellationToken, WaveSO targetWave)
        {
            if (targetWave.EnemyTypes == null || targetWave.EnemyTypes.Length == 0)
            {
                _currentWaveIndex++;
                OnWaveCompleted?.Invoke(this, _currentWaveIndex);

                if (_currentWaveIndex >= _wavesList.Count)
                {
                    OnAllWavesCompleted?.Invoke(this, EventArgs.Empty);
                    return;
                }

                ProcessWaves(cancellationToken).Forget();
                return;
            }

            foreach (var enemy in targetWave.EnemyTypes)
            {
                for (var i = 0; i < enemy.EnemyCount; i++)
                {
                    Enemy.SpawnEnemy(enemy.EnemyType, _pathManager.GetSpawnPosition());

                    if (i < enemy.EnemyCount - 1)
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(enemy.TimeBetweenSpawns),
                            cancellationToken: cancellationToken);
                    }
                }

                OnEnemyWaveSpawned?.Invoke(this, EventArgs.Empty);
            }
        }

        public int GetWaveIndex() =>
            _currentWaveIndex;

        public int GetTotalWavesCount() =>
            _wavesList.Count;

        public float GetCalldownTimer() =>
            _cooldownTimer;
    }
}