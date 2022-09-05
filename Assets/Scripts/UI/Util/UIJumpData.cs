using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using Mirror;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace UI.Util
{
    /// <summary>
    /// UI跳转信息存储单例类
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableSingleton/UIJumpData")]
    public class UIJumpData : ScriptableObject
    {
        [Header("开始界面")] [Scene] public string bootMenu;
        [Header("房间界面")] [Scene] public string roomMenu;
        [Header("加载界面")] [Scene] public string loadMenu;
        [Header("游戏界面")] [Scene] public string gameMenu;
    }
}