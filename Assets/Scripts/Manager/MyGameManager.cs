using System.Collections;
using System.Collections.Generic;
using Mirror;
using Player;
using UI.Util;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Manager
{
    /// <summary>
    /// 游戏管理单例类
    /// </summary>
    [DisallowMultipleComponent]
    public class MyGameManager : SingleTon<MyGameManager>
    {
        [Header("下一个要异步加载的场景")] [Scene] public string nextSceneToLoadAsync;

        [Header("UI跳转信息存储")] public UIJumpData uiJumpData;

        public List<MyPlayerController> allPlayers = new();

        [Header("玩家预制体")] public GameObject playerPrefab;

        public void AddPlayer()
        {
            var player = Instantiate(playerPrefab, gameObject.transform).GetComponent<MyPlayerController>();
            allPlayers.Add(player);
        }
        
        public void OnApplicationQuit()
        {
            MyWebSocket.MyWebSocket.Instance.Close();
        }
    }
}