using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;

namespace UI.Util
{
    /// <summary>
    /// UI跳转信息存储单例类
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableSingleton/UIJumpData")]
    public class UIJumpData : ScriptableSingleton<UIJumpData>
    {
        [Header("开始界面")] [Scene] public string startMenu;
        [Header("房间界面")] [Scene] public string roomMenu;
    }
}