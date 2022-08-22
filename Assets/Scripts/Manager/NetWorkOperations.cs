using System;
using System.Collections;
using System.Collections.Generic;
using MyWebSocket.Request;
using MyWebSocket.Response;
using Newtonsoft.Json;
using UI.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class NetWorkOperations : INetWorkOperations
    {
        private readonly Dictionary<string, RequestUtil> _requestPool = new();

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
                    SynchronousData(responseUtil.Data.ToString());
                    break;
                default:
                    throw new InvalidCastException();
            }
        }

        public void SynchronousData(string responseSynchronousData)
        {
            var playerListData = JsonConvert.DeserializeObject<PlayerListData>(responseSynchronousData);
            MyGameManager.Instance.PlayerListData = playerListData;
            MyGameManager.Instance.LocalPlayerInfo.ScriptName = playerListData?.ScriptName;

            if (SceneManager.GetActiveScene().name != MyGameManager.Instance.uiJumpData.roomMenu)
            {
                UIOperationUtil.GoToScene(MyGameManager.Instance.uiJumpData.roomMenu);
            }
        }

        public void ResponseErrorWork(ResponseUtil responseError)
        {
            if (!_requestPool.TryGetValue(responseError.RequestID, out var requestUtil))
            {
                return;
            }

            _requestPool.Remove(responseError.RequestID);
        }

        public void ResponseWork(ResponseUtil response)
        {
            if (!_requestPool.TryGetValue(response.RequestID, out var aRequestUtil))
            {
                return;
            }

            Debug.Log($"{response.Data}");

            if (!Enum.TryParse(aRequestUtil.NowRequestType, out RequestType nowTypeOfRequest))
            {
                return;
            }

            switch (nowTypeOfRequest)
            {
                case RequestType.Register:
                    var register = aRequestUtil as RequestRegister;
                    register?.CheckWorkDelegate(response.Data);
                    break;
                case RequestType.Login:
                    var login = aRequestUtil as RequestLogin;
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
                    var cancelMatching = aRequestUtil as RequestCancelMatching;
                    cancelMatching?.CheckWorkDelegate(response.Data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{nowTypeOfRequest.ToString()}");
            }

            _requestPool.Remove(response.RequestID);
        }
    }
}