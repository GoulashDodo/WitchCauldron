using System.Collections.Generic;

namespace Core.Run
{
    public class UnlockedRecipes
    {
        public IReadOnlyCollection<string> AllUnlockedRecipeIds => _unlockedRecipeIds;
        private readonly HashSet<string> _unlockedRecipeIds;

        public UnlockedRecipes(string[] initialUnlockedRecipeIds)
        {
            _unlockedRecipeIds = new HashSet<string>();

            if (initialUnlockedRecipeIds == null)
                return;

            foreach (var recipeId in initialUnlockedRecipeIds)
                UnlockCombination(recipeId);
        }
        
        public bool HasRecipe(string recipeId)
        {
            return !string.IsNullOrWhiteSpace(recipeId) && _unlockedRecipeIds.Contains(recipeId);
        }

        public void UnlockCombination(string recipeId)
        {
            if (string.IsNullOrWhiteSpace(recipeId))
                return;

            _unlockedRecipeIds.Add(recipeId);
        }
    }
}
