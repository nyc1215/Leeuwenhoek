using System;
using System.Collections.Generic;
using System.Linq;
using agora_gaming_rtc;
using Mirror;
using MyWebSocket.Response;
using Player;
using UI.Util;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
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

        private readonly Dictionary<int, int> _playerNumToImposterNum = new()
        {
            { 1, 0 }, { 2, 1 }, { 3, 1 }, { 4, 1 }, { 5, 2 }, { 6, 2 }
        };

        #endregion

        #region 网络操作

        public readonly NetWorkOperations NetWorkOperations = new();

        //安卓权限列表
        private readonly List<string> _permissionList = new();

        //IRtcEngine语音
        [Header("Agora语音appID")] public string rtcAppId = "YOUR_APPID";
        [Header("Agora语音token")] public string rtcToken = "";
        [Header("Agora语音频道名称")] public string rtcChannelName = "YOUR_CHANNEL_NAME";
        private IRtcEngine _rtcEngine;

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

        public void RandomSetImposter()
        {
            Assert.IsTrue(_playerNumToImposterNum.ContainsKey(allPlayers.Count));
            Debug.Log($"imposterNum = {_playerNumToImposterNum[allPlayers.Count]}");

            var imposterNum = 0;
            while (imposterNum < _playerNumToImposterNum[allPlayers.Count])
            {
                var playerToBeImposter = allPlayers[Random.Range(0, allPlayers.Count)];
                if (!playerToBeImposter.isImposter)
                {
                    playerToBeImposter.isImposter = true;
                    Debug.Log($"NetworkObjectId: {playerToBeImposter.NetworkObject.NetworkObjectId} become imposter");
                    imposterNum++;
                }
            }
        }

        #endregion

        #region MonoBehaviour

        private void OnApplicationQuit()
        {
            if (_rtcEngine == null)
            {
                return;
            }

            IRtcEngine.Destroy();
            _rtcEngine = null;
        }

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
            if (rtcAppId.Length > 10)
            {
                InitRtcEngine();
                VoiceJoinChannel();
            }
            else
            {
                Debug.LogWarning("Please fill in your appId in Canvas!!!!!");
            }
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

        public void ChangeSceneCallBack(Scene scene, LoadSceneMode mode)
        {
            foreach (var playerController in allPlayers)
            {
                playerController.ChangeSceneCallBack?.Invoke();
            }
        }

        #endregion

        #region 权限

        private void CheckPermission()
        {
#if UNITY_2018_3_OR_NEWER
            foreach (var permission in _permissionList.Where(permission =>
                         !Permission.HasUserAuthorizedPermission(permission)))
            {
                Permission.RequestUserPermission(permission);
            }
#endif
        }

        #endregion

        #region 语音

        private void InitRtcEngine()
        {
            _rtcEngine = IRtcEngine.GetEngine(rtcAppId);
            _rtcEngine.OnJoinChannelSuccess += OnJoinChannelSuccessHandler;
            _rtcEngine.OnLeaveChannel += OnLeaveChannelHandler;
            _rtcEngine.OnWarning += OnSDKWarningHandler;
            _rtcEngine.OnError += OnSDKErrorHandler;
            _rtcEngine.OnConnectionLost += OnConnectionLostHandler;
        }

        private void VoiceJoinChannel()
        {
            _rtcEngine.JoinChannelByKey(rtcToken, rtcChannelName);
            _rtcEngine.MuteAllRemoteAudioStreams(true);
            _rtcEngine.MuteLocalAudioStream(true);
        }

        public void VoiceChangeRemoteVoice(bool isMute)
        {
            _rtcEngine.MuteAllRemoteAudioStreams(isMute);
        }

        public void VoiceStartTalk()
        {
            _rtcEngine.MuteLocalAudioStream(false);
        }

        public void VoiceEndTalk()
        {
            _rtcEngine.MuteLocalAudioStream(true);
        }

        public void VoiceLeaveChannel()
        {
            Debug.Log("calling VoiceLeaveChannel");
            _rtcEngine?.LeaveChannel();
        }

        void OnJoinChannelSuccessHandler(string channelName, uint uid, int elapsed)
        {
            Debug.Log($"sdk version: {IRtcEngine.GetSdkVersion()}");
            Debug.Log($"onJoinChannelSuccess channelName: {channelName}, uid: {uid}, elapsed: {elapsed}");
        }

        void OnLeaveChannelHandler(RtcStats stats)
        {
            Debug.Log("OnLeaveChannelSuccess");
        }

        void OnSDKWarningHandler(int warn, string msg)
        {
            Debug.Log($"OnSDKWarning warn: {warn}, msg: {msg}");
        }

        void OnSDKErrorHandler(int error, string msg)
        {
            Debug.Log($"OnSDKError error: {error}, msg: {msg}");
        }

        void OnConnectionLostHandler()
        {
            Debug.Log("OnConnectionLost");
        }

        #endregion
    }
}