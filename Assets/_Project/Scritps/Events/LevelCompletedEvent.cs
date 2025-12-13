using IKhom.EventBusSystem.Runtime.abstractions;

namespace ChainDefense.Events
{
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
}