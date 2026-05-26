using Gameplay.Level;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace WitchCauldronEditorTools.Editor
{
    internal static class GameplayResultEditorUtility
    {
        private const string InstantWinMenuPath = "Tools/Witch Cauldron/Gameplay/Instant Win";
        private const string InstantLoseMenuPath = "Tools/Witch Cauldron/Gameplay/Instant Lose";

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
    }
}
