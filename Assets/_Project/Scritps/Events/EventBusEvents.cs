using ChainDefense.Enemies;
using ChainDefense.Towers;
using IKhom.EventBusSystem.Runtime.abstractions;

namespace ChainDefense.Events
{
    public struct GameOverEvent : IEvent
    {
        public readonly int LevelIndex;

        public GameOverEvent(int levelIndex) =>
            LevelIndex = levelIndex;
    }

    public struct LevelCompletedEvent : IEvent
    {
        public readonly int CompletedLevelIndex;
        public readonly int CompletedLevelNumber;

        public LevelCompletedEvent(int completedLevelIndex, int completedLevelNumber)
        {
            CompletedLevelNumber = completedLevelNumber;
            CompletedLevelIndex = completedLevelIndex;
        }
    }

    public struct ButtonClickedEvent : IEvent
    {
    }

    public struct TowerAttackEvent : IEvent
    {
        public readonly TowerAttackType AttackType;
        public readonly Enemy TargetEnemy;
        public readonly Tower Tower;

        public TowerAttackEvent(TowerAttackType attackType, Enemy targetEnemy, Tower tower)
        {
            AttackType = attackType;
            TargetEnemy = targetEnemy;
            Tower = tower;
        }
    }

    public struct WaveSpawnedEvent : IEvent
    {
    }

    public struct BaseTakeDamageEvent : IEvent
    {
    }

    public struct EnemyTakeDamageEvent : IEvent
    {
    }

    public struct EnemyDestroyedEvent : IEvent
    {
    }

    public struct ChainDestroyedEvent : IEvent
    {
    }

    public struct BallDragStartedEvent : IEvent
    {
    }

    public struct InvalidChainEvent : IEvent
    {
    }
    
    public struct EnemyAliveCountChangedEvent : IEvent
    {
        public readonly int AliveCount;
        public EnemyAliveCountChangedEvent(int aliveCount) =>
            AliveCount = aliveCount;
    }
    
}