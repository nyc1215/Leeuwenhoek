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

        /// <summary>
        /// 异步加载场景，并且显示加载进度场景
        /// </summary>
        /// <param name="sceneName">要加载的目标场景</param>
        /// <param name="loadSceneName">加载进度场景</param>
        /// <param name="clickToJump">是否需要点击后再跳转</param>
        private static IEnumerator IE_LoadSceneAsync(string sceneName, string loadSceneName, bool clickToJump)
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