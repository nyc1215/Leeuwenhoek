using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using BestHTTP.WebSocket;
using FairyGUI;
using Manager;
using UnityEngine.Assertions;

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

        public static void GoToScene(string sceneName)
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogWarning($"{sceneName} is null or empty!!!");
            }
        }

        public static void GoToSceneAsync(string sceneName)
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                MyGameManager.Instance.nextSceneToLoadAsync = sceneName;
                GoToScene(MyGameManager.Instance.uiJumpData.loadMenu);
            }
            else
            {
                Debug.LogWarning($"{sceneName} is null or empty!!!");
            }
        }
    }
}