using System;
using System.Collections.Generic;
using System.Threading;
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
        public static WaveManager Instance { get; private set; }

        [SerializeField] private List<WaveSO> _wavesList;

        private int _currentWaveIndex;
        private int _enemiesAlive;
        private PathManager _pathManager;

        private void Awake() =>
            Instance = this;

        private void Start()
        {
            _pathManager = PathManager.Instance;

            Enemy.OnEnemyDestroyed += Enemy_OnEnemyDestroyed;
            
            StartWave(CancellationToken.None).Forget();
        }

        private void Enemy_OnEnemyDestroyed(object sender, EventArgs e)
        {
            _enemiesAlive--;

            OnEnemyNumberChanged?.Invoke(this, _enemiesAlive);

            if (_enemiesAlive > 0)
                return;

            _currentWaveIndex++;
            OnWaveCompleted?.Invoke(this, _currentWaveIndex);
            StartWave(CancellationToken.None).Forget();
        }

        public async UniTask StartWave(CancellationToken cancellationToken)
        {
            if (_currentWaveIndex >= _wavesList.Count)
            {
                return;
            }

            var currentWave = _wavesList[_currentWaveIndex];
            
            await UniTask.Delay(
                TimeSpan.FromSeconds(currentWave.DelayBeforeStarting),
                cancellationToken: cancellationToken
            );

            foreach (var wave in currentWave.Waves)
            {
                for (int i = 0; i < wave.EnemyCount; i++)
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

        private void SpawnEnemy(EnemySO waveEnemyType)
        {
            Enemy.SpawnEnemy(waveEnemyType, _pathManager.GetSpawnPosition());
            _enemiesAlive++;
            OnEnemyNumberChanged?.Invoke(this, _enemiesAlive);
        }
    }
}