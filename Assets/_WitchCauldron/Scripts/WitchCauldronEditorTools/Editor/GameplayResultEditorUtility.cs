using Gameplay.Level;
using Core.Run;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace WitchCauldronEditorTools.Editor
{
    internal static class GameplayResultEditorUtility
    {
        private const string InstantWinMenuPath = "Tools/Witch Cauldron/Gameplay/Instant Win _F9";
        private const string InstantLoseMenuPath = "Tools/Witch Cauldron/Gameplay/Instant Lose _F8";
        private const string AddMoneyMenuPath = "Tools/Witch Cauldron/Run/Add 9999 Money";
        private const int MoneyAmount = 9999;

        [MenuItem(InstantWinMenuPath, priority = 100)]
        private static void InstantWin()
        {
            Execute(game => game.ForceWin(), "instant win");
        }

        [MenuItem(InstantWinMenuPath, true)]
        private static bool CanInstantWin()
        {
            return CanExecute();
        }

        [MenuItem(InstantLoseMenuPath, priority = 101)]
        private static void InstantLose()
        {
            Execute(game => game.ForceLose(), "instant lose");
        }

        [MenuItem(InstantLoseMenuPath, true)]
        private static bool CanInstantLose()
        {
            return CanExecute();
        }

        [MenuItem(AddMoneyMenuPath, priority = 200)]
        private static void AddMoney()
        {
            if (!EditorApplication.isPlaying)
            {
                Debug.LogWarning("Cannot add money: enter Play Mode first.");
                return;
            }

            if (!TryGetRunState(out var runState))
            {
                Debug.LogWarning("Cannot add money: run state was not found.");
                return;
            }

            runState.Wallet.Add(MoneyAmount);
            Debug.Log($"Added {MoneyAmount} money. Current balance: {runState.Wallet.Balance}.");
        }

        [MenuItem(AddMoneyMenuPath, true)]
        private static bool CanAddMoney()
        {
            return EditorApplication.isPlaying && TryGetRunState(out _);
        }

        private static bool CanExecute()
        {
            return EditorApplication.isPlaying && TryGetGameplay(out _);
        }

        private static void Execute(System.Action<G> action, string actionName)
        {
            if (!EditorApplication.isPlaying)
            {
                Debug.LogWarning($"Cannot execute {actionName}: enter Play Mode first.");
                return;
            }

            if (!TryGetGameplay(out var game))
            {
                Debug.LogWarning($"Cannot execute {actionName}: gameplay context was not found in loaded scenes.");
                return;
            }

            action.Invoke(game);
        }

        private static bool TryGetGameplay(out G game)
        {
            foreach (var sceneContext in Resources.FindObjectsOfTypeAll<SceneContext>())
            {
                if (sceneContext == null ||
                    sceneContext.Container == null ||
                    EditorUtility.IsPersistent(sceneContext) ||
                    !sceneContext.gameObject.scene.isLoaded)
                {
                    continue;
                }

                game = sceneContext.Container.TryResolve<G>();
                if (game != null)
                    return true;
            }

            game = null;
            return false;
        }

        private static bool TryGetRunState(out RunState runState)
        {
            if (ProjectContext.HasInstance && TryResolve(ProjectContext.Instance, out runState))
                return true;

            foreach (var sceneContext in Resources.FindObjectsOfTypeAll<SceneContext>())
            {
                if (TryResolve(sceneContext, out runState))
                    return true;
            }

            runState = null;
            return false;
        }

        private static bool TryResolve<T>(Context context, out T resolved)
            where T : class
        {
            if (context == null ||
                context.Container == null ||
                EditorUtility.IsPersistent(context) ||
                !context.gameObject.scene.isLoaded)
            {
                resolved = null;
                return false;
            }

            resolved = context.Container.TryResolve<T>();
            return resolved != null;
        }
    }
}
