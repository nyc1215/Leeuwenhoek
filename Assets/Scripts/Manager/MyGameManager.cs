using System.Collections;
using System.Collections.Generic;
using Mirror;
using UI.Util;
using Unity.Collections;
using UnityEngine;

namespace Manager
{
    /// <summary>
    /// 游戏管理单例类
    /// </summary>
    [DisallowMultipleComponent]
    public class MyGameManager : SingleTon<MyGameManager>
    {
        [Header("下一个要异步加载的场景")] [Scene]
        public string nextSceneToLoadAsync;
        
        [Header("UI跳转信息存储")]
        public UIJumpData uiJumpData;
    }
}