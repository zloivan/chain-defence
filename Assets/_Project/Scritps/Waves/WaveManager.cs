using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ChainDefense.Enemies;
using ChainDefense.PathFinding;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IKhom.StateMachineSystem.Runtime
{
    public class WaveManager : MonoBehaviour
    {
        public event EventHandler<int> OnWaveCompleted;
        public event EventHandler OnEnemyWaveSpawned;
        public event EventHandler<int> OnEnemyNumberChanged;
        public event EventHandler AllWavesCompleted;
        public event EventHandler<float> OnWaveCooldownChanged;

        public static WaveManager Instance { get; private set; }

        [SerializeField] private List<WaveSO> _wavesList;

        private int _currentWaveIndex;
        private int _enemiesAlive;
        private PathManager _pathManager;
        private float _cooldownTimer;

        private void Awake() =>
            Instance = this;

        private void Start()
        {
            _pathManager = PathManager.Instance;

            Enemy.OnEnemyDestroyed += Enemy_OnEnemyDestroyed;

            ProcessWaves(CancellationToken.None).Forget();
        }

        private void Enemy_OnEnemyDestroyed(object sender, EventArgs e)
        {
            _enemiesAlive--;

            OnEnemyNumberChanged?.Invoke(this, _enemiesAlive);

            if (_enemiesAlive > 0)
                return;

            _currentWaveIndex++;
            OnWaveCompleted?.Invoke(this, _currentWaveIndex);

            if (_currentWaveIndex >= _wavesList.Count)
            {
                AllWavesCompleted?.Invoke(this, EventArgs.Empty);
                return;
            }

            ProcessWaves(CancellationToken.None).Forget();
        }

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
            foreach (var wave in targetWave.Waves)
            {
                for (var i = 0; i < wave.EnemyCount; i++)
                {
                    SpawnEnemy(wave.EnemyType);

                    if (i < wave.EnemyCount - 1)
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(wave.TimeBetweenSpawns),
                            cancellationToken: cancellationToken);
                    }
                }

                OnEnemyWaveSpawned?.Invoke(this, EventArgs.Empty);
            }
        }

        public int GetNumberOfEnemiesAlive() =>
            _enemiesAlive;

        public int GetWaveIndex() =>
            _currentWaveIndex;

        public int GetTotalWavesCount() =>
            _wavesList.Count;

        public float GetCalldownTimer() =>
            _cooldownTimer;

        private void SpawnEnemy(EnemySO waveEnemyType)
        {
            Enemy.SpawnEnemy(waveEnemyType, _pathManager.GetSpawnPosition());
            _enemiesAlive++;
            OnEnemyNumberChanged?.Invoke(this, _enemiesAlive);
        }
    }
}