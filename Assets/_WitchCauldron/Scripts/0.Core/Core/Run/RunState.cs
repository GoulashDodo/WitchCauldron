using System;
using Core.SO;
using Gameplay._root.SO;
using Gameplay.Level.SO;

namespace Core.Run
{
    public class RunState
    {
        public int CurrentLevelIndex { get; private set; } = -1;
        public string CurrentLevelId { get; private set; }
        public string CurrentLevelName
        {
            get
            {
                var levels = _levelSettings.AllSettings;
                return levels[CurrentLevelIndex].LevelName;
            }
        }
        
        public bool IsCompleted { get; private set; }
        
        public bool HasCurrentLevel
        {
            get
            {
                var levels = _levelSettings.AllSettings;
                return !IsCompleted && levels != null && CurrentLevelIndex >= 0 && CurrentLevelIndex < levels.Length;
            }
        }

        private readonly AllLevelSettings _levelSettings;

        private readonly UnlockedSelectableItems _unlockedSelectableItems;
        private readonly UnlockedRecipes _unlockedRecipes;

        public UnlockedSelectableItems UnlockedSelectableItems => _unlockedSelectableItems;
        public UnlockedRecipes UnlockedRecipes => _unlockedRecipes;
        
        
        public RunState(GameSettings settings)
        {
            var gameplaySettings = settings.GameplaySettings;
            _levelSettings = gameplaySettings.AllLevelSettings;

            var macroSettings = settings.HutSettings != null ? settings.HutSettings.MacroSettings : null;
            _unlockedSelectableItems = new UnlockedSelectableItems(macroSettings?.InitialSelectableItemsIds);
            _unlockedRecipes = new UnlockedRecipes(macroSettings?.InitialRecipeIds);
        }

        public void ApplyUnlockRewards(UnlockReward[] rewards)
        {
            if (rewards == null)
                return;

            foreach (var reward in rewards)
                ApplyUnlockReward(reward);
        }

        public void ApplyUnlockReward(UnlockReward reward)
        {
            switch (reward.Type)
            {
                case UnlockRewardType.SelectableItem:
                    _unlockedSelectableItems.UnlockNewItem(reward.UnlockId);
                    break;
                case UnlockRewardType.Recipe:
                    _unlockedRecipes.UnlockCombination(reward.UnlockId);
                    break;
            }
        }

        public bool StartNewRun()
        {
            IsCompleted = false;

            var levels = _levelSettings.AllSettings;

            if (levels == null || levels.Length == 0 || levels[0] == null)
            {
                CurrentLevelIndex = -1;
                CurrentLevelId = null;
                IsCompleted = true;
                return false;
            }

            SetCurrentLevel(0, levels[0]);
            return true;
        }

        public bool TrySetCurrentLevel(string levelId)
        {
            var levels = _levelSettings.AllSettings;

            if (levels == null)
                return false;

            var levelIndex = Array.FindIndex(
                levels,
                settings => settings != null && settings.LevelId == levelId
            );

            if (levelIndex < 0)
                return false;

            IsCompleted = false;
            SetCurrentLevel(levelIndex, levels[levelIndex]);
            return true;
        }

        public bool TrySetNextLevel()
        {
            var levels = _levelSettings.AllSettings;

            if (levels == null)
            {
                IsCompleted = true;
                return false;
            }

            var currentIndex = CurrentLevelIndex >= 0
                ? CurrentLevelIndex
                : Array.FindIndex(levels, settings => settings != null && settings.LevelId == CurrentLevelId);

            var nextIndex = currentIndex + 1;

            if (currentIndex < 0 || nextIndex >= levels.Length || levels[nextIndex] == null)
            {
                IsCompleted = true;
                return false;
            }

            SetCurrentLevel(nextIndex, levels[nextIndex]);
            return true;
        }

        private void SetCurrentLevel(int index, LevelSettings levelSettings)
        {
            CurrentLevelIndex = index;
            CurrentLevelId = levelSettings.LevelId;
        }
    }
}
