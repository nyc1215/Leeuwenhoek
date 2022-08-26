using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.JSON;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.X509;
using Mirror;
using MyWebSocket.Request;
using MyWebSocket.Response;
using Newtonsoft.Json;
using Player;
using UI.Util;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Manager
{
    public struct LocalPlayerInfo
    {
        public string GroupId;
        public string Account;
        public string ScriptName;
        public bool ReadyForGame;

        public LocalPlayerInfo(string groupId, string account, string scriptName, bool readyForGame)
        {
            GroupId = groupId;
            Account = account;
            ScriptName = scriptName;
            ReadyForGame = readyForGame;
        }
    }

    public struct JoyStickOutputXY
    {
        public float _x;
        public float _y;

        public JoyStickOutputXY(float x, float y)
        {
            _x = x;
            _y = y;
        }

        public bool IsZero()
        {
            return _x == 0f && _y == 0f;
        }

        public Vector2 GetVector2()
        {
            return new Vector2(_x, _y);
        }
    }

    /// <summary>
    /// 游戏管理单例类
    /// </summary>
    [DisallowMultipleComponent]
    public class MyGameManager : SingleTon<MyGameManager>
    {
        #region UI与场景相关变量

        [Header("下一个要异步加载的场景")] [Scene] public string nextSceneToLoadAsync;

        [Header("UI跳转信息存储")] public UIJumpData uiJumpData;

        #endregion

        #region 玩家相关变量

        public LocalPlayerInfo LocalPlayerInfo = new("", "", "剧本杀", false);
        public PlayerListData PlayerListData;
        [Header("玩家列表")]public List<MyPlayerController> allPlayers = new();

        #endregion

        #region 网络操作

        public readonly NetWorkOperations NetWorkOperations = new();

        #endregion

        #region 玩家控制

        public void SendJoyStickDegreeToPlayers(JoyStickOutputXY outputXY)
        {
            foreach (var playerController in allPlayers)
            {
                if (Vector2.SqrMagnitude(outputXY.GetVector2()) <= float.Epsilon)
                {
                    outputXY = new JoyStickOutputXY(0f, 0f);
                }

                playerController.JoyStickOutput = outputXY;
            }
        }

        #endregion

        #region MonoBehaviour

        protected override void Awake()
        {
            base.Awake();

            Debug.unityLogger.logEnabled = true;
        }

        #endregion

        #region 场景

        public static bool CompareScene(string scene)
        {
            return SceneManager.GetActiveScene().path == scene;
        }

        public void GameStart()
        {
            UIOperationUtil.GoToSceneAsync(uiJumpData.gameMenu);
        }

        #endregion
    }
}