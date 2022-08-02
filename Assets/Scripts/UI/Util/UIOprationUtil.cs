using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Util
{
    /// <summary>
    /// UI操作基础类
    /// </summary>
    public static class UIOperationUtil
    {
        public static void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public static void GoToScene(string scene)
        {
            if (!string.IsNullOrEmpty(scene))
            {
                SceneManager.LoadScene(scene);
            }
            else
            {
                Debug.LogWarning($"{scene} is null or empty!!!");
            }
        }
    }
}

