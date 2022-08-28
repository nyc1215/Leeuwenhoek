using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using MyWebSocket.Response;
using Player;
using UI.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_2018_3_OR_NEWER
using UnityEngine.Android;
#endif

namespace Manager
{
    public struct LocalPlayerInfo
    {
        public string GroupId;
        public string Account;
        public string AccountName;
        public string ScriptName;
        public bool ReadyForGame;

        public LocalPlayerInfo(string groupId, string account, string accountName, string scriptName, bool readyForGame)
        {
            GroupId = groupId;
            Account = account;
            AccountName = accountName;
            ScriptName = scriptName;
            ReadyForGame = readyForGame;
        }
    }

    public readonly struct JoyStickOutputXY
    {
        private readonly float _x;
        private readonly float _y;

        public JoyStickOutputXY(float x, float y)
        {
            _x = x;
            _y = y;
        }

        public bool IsZero()
        {
            return Mathf.Abs(_x) <= float.Epsilon && Mathf.Abs(_y) <= float.Epsilon;
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

        public LocalPlayerInfo LocalPlayerInfo = new("", "", "", "剧本杀", false);
        public PlayerListData PlayerListData;
        public MyPlayerController localPlayerController;
        public MyPlayerNetwork localPlayerNetwork;
        [Header("玩家列表")] public List<MyPlayerController> allPlayers = new();

        #endregion

        #region 网络操作

        public readonly NetWorkOperations NetWorkOperations = new();

        //安卓权限列表
        private readonly List<string> _permissionList = new();

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

        private void Start()
        {
#if UNITY_2018_3_OR_NEWER
            _permissionList.Add(Permission.Microphone);
#endif
        }

        private void Update()
        {
#if UNITY_2018_3_OR_NEWER
            // 获取设备权限。 
            CheckPermission();
#endif
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

        #region 权限

        private void CheckPermission()
        {
#if UNITY_2018_3_OR_NEWER
            foreach (var permission in _permissionList.Where(permission => !Permission.HasUserAuthorizedPermission(permission)))
            {
                Permission.RequestUserPermission(permission);
            }
#endif
        }

        #endregion
    }
}