using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.JSON;
using Mirror;
using MyWebSocket.Request;
using MyWebSocket.Response;
using Newtonsoft.Json;
using Player;
using UI.Util;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Manager
{
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

        public string account = "";
        public string scriptName = "剧本杀";

        public List<MyPlayerController> allPlayers = new();

        [Header("玩家预制体")] public GameObject playerPrefab;

        #endregion

        #region 网络相关变量

        private readonly Dictionary<string, RequestUtil> _requestPool = new();

        #endregion

        #region 玩家控制

        public void AddPlayer()
        {
            var player = Instantiate(playerPrefab, gameObject.transform).GetComponent<MyPlayerController>();
            allPlayers.Add(player);
        }

        #endregion

        #region 服务器通信

        public void SendRequest(RequestUtil request)
        {
            if (MyWebSocket.MyWebSocket.Instance.WebSocket.IsOpen)
            {
                MyWebSocket.MyWebSocket.Instance.Send(request.ToJson());
                if (request.NowRequestType == RequestType.Invite.ToString() ||
                    request.NowRequestType == RequestType.AddRoom.ToString())
                {
                    return;
                }

                _requestPool.Add(request.RequestID, request);
            }
            else
            {
                Debug.LogWarning("SendRequest error!!!");
            }
        }


        public void GetResponse(string json)
        {
            ResponseUtil responseUtil = new(json);
            if (responseUtil.NowResponseType == "NULL")
            {
                Debug.Log($"{json} is not json");
                return;
            }

            if (!Enum.TryParse(responseUtil.NowResponseType, out ResponseType nowTypeOfResponse))
            {
                Debug.Log("string to ResponseType failed");
                return;
            }

            switch (nowTypeOfResponse)
            {
                case ResponseType.Error:
                    ResponseErrorWork(responseUtil);
                    break;
                case ResponseType.Response:
                    ResponseWork(responseUtil);
                    break;
                case ResponseType.SynchronousData:
                    SynchronousData(responseUtil);
                    break;
                default:
                    throw new InvalidCastException();
            }
        }

        private void SynchronousData(ResponseUtil responseSynchronousData)
        {
            Debug.Log(responseSynchronousData.Data);
        }

        private void ResponseErrorWork(ResponseUtil responseError)
        {
            if (!_requestPool.TryGetValue(responseError.RequestID, out var requestUtil))
            {
                return;
            }

            _requestPool.Remove(responseError.RequestID);
        }

        private void ResponseWork(ResponseUtil response)
        {
            if (!_requestPool.TryGetValue(response.RequestID, out var requestUtil))
            {
                return;
            }

            Debug.Log($"{response.Data}");

            var request = _requestPool[response.RequestID];
            if (!Enum.TryParse(request.NowRequestType, out RequestType nowTypeOfRequest))
            {
                return;
            }

            switch (nowTypeOfRequest)
            {
                case RequestType.Register:
                    var register = request as RequestRegister;
                    register?.CheckWorkDelegate(response.Data);
                    break;
                case RequestType.Login:
                    var login = request as RequestLogin;
                    login?.CheckWorkDelegate(response.Data);
                    break;
                case RequestType.ListOfScripts:
                    break;
                case RequestType.Matching:
                    break;
                case RequestType.CreateRoom:
                    break;
                case RequestType.Invite:
                    break;
                case RequestType.AddRoom:
                    break;
                case RequestType.SubmitLead:
                    break;
                case RequestType.Settlement:
                    break;
                case RequestType.SubmitScript:
                    break;
                case RequestType.SendMessage:
                    break;
                case RequestType.CancelMatching:
                    var cancelMatching = request as RequestCancelMatching;
                    cancelMatching?.CheckWorkDelegate(response.Data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{nowTypeOfRequest.ToString()}");
            }

            _requestPool.Remove(response.RequestID);
        }

        #endregion
    }
}