using ChainDefense.Towers;
using IKhom.EventBusSystem.Runtime.abstractions;

namespace ChainDefense.Events
{
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

    public struct TowerAttackEvent : IEvent
    {
        public readonly TowerAttackType AttackType;

        public TowerAttackEvent(TowerAttackType attackType)
        {
            AttackType = attackType;
        }
    }

    public struct ButtonClickedEvent : IEvent
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
}