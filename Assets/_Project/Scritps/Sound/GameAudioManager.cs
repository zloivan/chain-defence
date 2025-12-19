using ChainDefense.Events;
using ChainDefense.Towers;
using IKhom.EventBusSystem.Runtime;
using IKhom.SoundSystem.Runtime.components;
using IKhom.SoundSystem.Runtime.data;
using IKhom.UtilitiesLibrary.Runtime.components;
using UnityEngine;
using UnityEngine.Serialization;

namespace ChainDefense.Sound
{
    public class GameAudioManager : SingletonBehaviour<GameAudioManager>
    {
        [Header("Sound Manager")]
        [SerializeField] private SoundManager _soundManager;
        
        [Header("UI Sounds")]
        [SerializeField] private SoundData _buttonClickSound;
        
        [Header("Gameplay Sounds")]
        [SerializeField] private SoundData _ballSelectSound;
        [SerializeField] private SoundData _chainCompleteSound;
        [SerializeField] private SoundData _invalidChainSound;
        [SerializeField] private SoundData _towerBuildSound;
        
        [Header("Combat Sounds")]
        [SerializeField] private SoundData _towerShootFireSound;
        [SerializeField] private SoundData _towerShootIceSound;
        [FormerlySerializedAs("_towerShootPoisonSound")] [SerializeField] private SoundData _towerShootExplodeSound;
        [SerializeField] private SoundData _towerShootLightningSound;
        [SerializeField] private SoundData _enemyHitSound;
        [SerializeField] private SoundData _enemyDeathSound;
        
        [Header("Base Sounds")]
        [SerializeField] private SoundData _baseDamageSound;
        [SerializeField] private SoundData _victorySound;
        [SerializeField] private SoundData _defeatSound;
        [SerializeField] private SoundData _waveStartSound;

        
        // Event bindings
        private EventBinding<BallDragStartedEvent> _ballDragStartedBinding;
        private EventBinding<ChainDestroyedEvent> _chainDestroyedBinding;
        private EventBinding<ButtonClickedEvent> _buttonClickedBinding;
        private EventBinding<TowerAttackEvent> _towerAttackBinding;
        private EventBinding<EnemyDestroyedEvent> _enemyDestroyedBinding;
        private EventBinding<EnemyTakeDamageEvent> _enemyTakeDamageBinding;
        private EventBinding<BaseTakeDamageEvent> _baseTakeDamageBinding;
        private EventBinding<GameOverEvent> _gameOverBinding;
        private EventBinding<WaveSpawnedEvent> _waveSpawnedBinding;
        private EventBinding<LevelCompletedEvent> _levelCompletedBinding;
        private EventBinding<InvalidChainEvent> _invalidChainBinding;

        private void Start()
        {
            _ballDragStartedBinding = new EventBinding<BallDragStartedEvent>(OnBallDragStarted);
            EventBus<BallDragStartedEvent>.Register(_ballDragStartedBinding);
            
            _chainDestroyedBinding = new EventBinding<ChainDestroyedEvent>(OnChainDestroyed);
            EventBus<ChainDestroyedEvent>.Register(_chainDestroyedBinding);
            
            _buttonClickedBinding = new EventBinding<ButtonClickedEvent>(OnButtonClicked);
            EventBus<ButtonClickedEvent>.Register(_buttonClickedBinding);
            
            // Towers
            _towerAttackBinding = new EventBinding<TowerAttackEvent>(OnTowerAttack);
            EventBus<TowerAttackEvent>.Register(_towerAttackBinding);
            
            // Enemies
            _enemyDestroyedBinding = new EventBinding<EnemyDestroyedEvent>(OnEnemyDestroyed);
            EventBus<EnemyDestroyedEvent>.Register(_enemyDestroyedBinding);
            
            _enemyTakeDamageBinding = new EventBinding<EnemyTakeDamageEvent>(OnEnemyTakeDamage);
            EventBus<EnemyTakeDamageEvent>.Register(_enemyTakeDamageBinding);
            
            // Base & Waves
            _baseTakeDamageBinding = new EventBinding<BaseTakeDamageEvent>(OnBaseTakeDamage);
            EventBus<BaseTakeDamageEvent>.Register(_baseTakeDamageBinding);
            
            _gameOverBinding = new EventBinding<GameOverEvent>(OnGameOver);
            EventBus<GameOverEvent>.Register(_gameOverBinding);
            
            _waveSpawnedBinding = new EventBinding<WaveSpawnedEvent>(OnWaveSpawned);
            EventBus<WaveSpawnedEvent>.Register(_waveSpawnedBinding);
            
            _levelCompletedBinding = new EventBinding<LevelCompletedEvent>(OnLevelCompleted);
            EventBus<LevelCompletedEvent>.Register(_levelCompletedBinding);
            
            _invalidChainBinding = new EventBinding<InvalidChainEvent>(OnInvalidChain);
            EventBus<InvalidChainEvent>.Register(_invalidChainBinding);
        }
        
        private void OnBallDragStarted(BallDragStartedEvent evt) => PlaySound(_ballSelectSound);
        private void OnChainDestroyed(ChainDestroyedEvent evt) => PlaySound(_chainCompleteSound);
        private void OnButtonClicked(ButtonClickedEvent evt) => PlaySound(_buttonClickSound);
        private void OnEnemyDestroyed(EnemyDestroyedEvent evt) => PlaySound(_enemyDeathSound);
        private void OnEnemyTakeDamage(EnemyTakeDamageEvent evt) => PlaySound(_enemyHitSound);
        private void OnBaseTakeDamage(BaseTakeDamageEvent evt) => PlaySound(_baseDamageSound);
        private void OnGameOver(GameOverEvent evt) => PlaySound(_defeatSound);
        private void OnWaveSpawned(WaveSpawnedEvent evt) => PlaySound(_waveStartSound);
        private void OnLevelCompleted(LevelCompletedEvent evt) => PlaySound(_victorySound);
        private void OnInvalidChain(InvalidChainEvent evt) => PlaySound(_invalidChainSound);

        
        private void OnTowerAttack(TowerAttackEvent evt)
        {
            var soundData = evt.AttackType switch
            {
                TowerAttackType.SingleTarget => _towerShootFireSound,
                TowerAttackType.Slow => _towerShootIceSound,
                TowerAttackType.AOE => _towerShootExplodeSound,
                TowerAttackType.Chain => _towerShootLightningSound,
                _ => null
            };

            if (soundData != null)
                PlaySound(soundData, randomizePitch: true);
        }

        private void PlaySound(SoundData soundData, bool randomizePitch = false)
        {
            if (soundData == null || _soundManager == null) return;

            var builder = _soundManager.CreateSound().WithSoundData(soundData);
            
            if (randomizePitch)
                builder.WithRandomPitch();
            
            builder.Play();
        }
    }
}