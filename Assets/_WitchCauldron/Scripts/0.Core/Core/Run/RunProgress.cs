using System;

namespace Core.Run
{
    public class RunProgress
    {
        public event Action<int> HighestCompletedLevelIndexChanged;

        public int HighestCompletedLevelIndex { get; private set; } = -1;

        public void MarkLevelCompleted(int levelIndex)
        {
            if (levelIndex <= HighestCompletedLevelIndex)
                return;

            HighestCompletedLevelIndex = levelIndex;
            HighestCompletedLevelIndexChanged?.Invoke(HighestCompletedLevelIndex);
        }
    }
}
