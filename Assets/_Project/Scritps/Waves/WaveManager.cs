using System;
using System.Collections.Generic;
using System.Threading;
using ChainDefense.Enemies;
using ChainDefense.Events;
using ChainDefense.PathFinding;
using Cysharp.Threading.Tasks;
using IKhom.EventBusSystem.Runtime;
using IKhom.ServiceLocatorSystem.Runtime;
using UnityEngine;

namespace ChainDefense.Waves
{
    public class WaveManager : MonoBehaviour
    {
        public event EventHandler<float> OnWaveCooldownChanged;
        public event EventHandler OnWavesListUpdate;

        [SerializeField] private List<WaveSO> _wavesList;
        [SerializeField] private bool _useMockWaves;

        private int _currentWaveIndex;
        private PathManager _pathManager;
        private float _delayBeforeWaveTimer;
        private CancellationTokenSource _waveSequenceCts;
        private EnemySpawner _enemySpawner;
        private EventBinding<EnemyDestroyedEvent> _eventBinding;

        private void Start()
        {
            _pathManager = ServiceLocator.ForSceneOf(this).Get<PathManager>();
            _enemySpawner = ServiceLocator.ForSceneOf(this).Get<EnemySpawner>();
            _eventBinding = new EventBinding<EnemyDestroyedEvent>(Enemy_OnDestroyed);
            EventBus<EnemyDestroyedEvent>.Register(_eventBinding);
            
            if (_useMockWaves)
            {
                RunWaveSequence().Forget();
            }
        }

        private void OnDestroy()
        {
            _waveSequenceCts?.Cancel();
            _waveSequenceCts?.Dispose();

            EventBus<EnemyDestroyedEvent>.Deregister(_eventBinding);
        }

        public void SetupWavesList(List<WaveSO> wavesList)
        {
            _wavesList = wavesList;
            OnWavesListUpdate?.Invoke(this, EventArgs.Empty);
            _currentWaveIndex = 0;
        }

        private void Enemy_OnDestroyed()
        {
            if (_enemySpawner.GetAliveCount() > 0)
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
                linkedCts.Dispose();
                return;
            }

            while (_currentWaveIndex < _wavesList.Count)
            {
                var currentWave = _wavesList[_currentWaveIndex];

                cancelled = await RunWaveCooldown(currentWave.DelayBeforeStarting, linkedCts.Token);
                if (cancelled)
                {
                    linkedCts.Dispose();
                    return;
                }

                cancelled = await SpawnWave(currentWave, linkedCts.Token);
                if (cancelled)
                {
                    linkedCts.Dispose();
                    return;
                }

                if (currentWave.EnemyTypes == null || currentWave.EnemyTypes.Length == 0)
                {
                    continue;
                }

                linkedCts.Dispose();
                return;
            }

            linkedCts.Dispose();
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
                    _enemySpawner.SpawnEnemy(enemyGroup.EnemyType, _pathManager.GetSpawnPosition());

                    if (i < enemyGroup.EnemyCount - 1)
                    {
                        var cancelled = await UniTask.Delay(
                            TimeSpan.FromSeconds(enemyGroup.TimeBetweenSpawns),
                            cancellationToken: cancellationToken
                        ).SuppressCancellationThrow();

                        if (cancelled) 
                            return true;
                    }
                }

                EventBus<WaveSpawnedEvent>.Raise(new WaveSpawnedEvent());
            }

            return false;
        }

        private void CompleteCurrentWave()
        {
            _currentWaveIndex++;

            if (_currentWaveIndex >= _wavesList.Count)
            {
                EventBus<AllWavesCompletedEvent>.Raise(new AllWavesCompletedEvent());
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

        public void Clear()
        {
            _currentWaveIndex = 0;
            _waveSequenceCts?.Cancel();
            OnWavesListUpdate?.Invoke(this, EventArgs.Empty);
        }
    }
}