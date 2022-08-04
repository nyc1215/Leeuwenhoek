using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using Manager;

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

        public static void GoToSceneAsync(string scene)
        {
            if (!string.IsNullOrEmpty(scene))
            {
                MyGameManager.Instance.StartCoroutine(IE_LoadSceneAsync(scene));
            }
            else
            {
                Debug.LogWarning($"{scene} is null or empty!!!");
            }
        }

        private static IEnumerator IE_LoadSceneAsync(string sceneName)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            asyncOperation.allowSceneActivation = false;

            while (!asyncOperation.isDone)
            {
                var progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
                Debug.Log($"Loading {sceneName} progress:" + (progress * 100) + "%");
            }

            if (Mathf.Approximately(asyncOperation.progress, 0.9f))
            {
                Debug.Log($"Scene {sceneName} Almost loaded!");
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}