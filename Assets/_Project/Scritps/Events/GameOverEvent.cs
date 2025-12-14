using IKhom.EventBusSystem.Runtime.abstractions;

namespace ChainDefense.Events
{
    public struct GameOverEvent : IEvent
    {
        public readonly int LevelIndex;

        public GameOverEvent(int levelIndex) =>
            LevelIndex = levelIndex;
    }
}