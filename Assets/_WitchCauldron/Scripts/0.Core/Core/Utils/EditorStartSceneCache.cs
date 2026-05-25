using UnityEngine.SceneManagement;

namespace Core.Utils
{
    public static class EditorStartSceneCache
    {
#if UNITY_EDITOR
        public static string RequestedSceneName { get; private set; }

        [UnityEditor.InitializeOnEnterPlayMode]
        private static void OnEnterPlayMode()
        {
            RequestedSceneName = SceneManager.GetActiveScene().name;
        }
#endif
    }
}