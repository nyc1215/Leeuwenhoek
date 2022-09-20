using System;
using System.Collections.Generic;
using System.Linq;
using MyWebSocket.Request;
using MyWebSocket.Response;
using Newtonsoft.Json;
using UI.Boot;
using UI.Room;
using UI.Util;
using UnityEngine;

namespace Manager
{
    public class NetWorkOperations : INetWorkOperations
    {
        private readonly Dictionary<string, RequestUtil> _requestPool = new();

        public void SendRequest(RequestUtil request)
        {
            if (MyWebSocket.MyWebSocket.WebSocket.IsOpen)
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
            if (responseSynchronousData.Contains("playerList", StringComparison.Ordinal))
            {
                if (!responseSynchronousData.Contains(MyGameManager.Instance.LocalPlayerInfo.Account, StringComparison.Ordinal))
                {
                    return;
                }

                var playerListData = JsonConvert.DeserializeObject<PlayerListData>(responseSynchronousData);
                if (playerListData == null)
                {
                    return;
                }

                var existLocalPlayer = false;
                foreach (var _ in playerListData.PlayerList.Where(playerListNode => playerListNode.Account == MyGameManager.Instance.LocalPlayerInfo.Account))
                {
                    existLocalPlayer = true;
                }

                MyGameManager.Instance.PlayerListData = playerListData;
                MyGameManager.Instance.LocalPlayerInfo.ScriptName = playerListData?.ScriptName;
                MyGameManager.Instance.LocalPlayerInfo.GroupId = playerListData?.GroupId;

                if (!existLocalPlayer)
                {
                    return;
                }

                if (!MyGameManager.CompareScene(MyGameManager.Instance.uiJumpData.roomMenu))
                {
                    BootUIPanel.ChoosePanelComponent.visible = false;
                    BootUIPanel.InfoPanelComponent.visible = false;
                    MyGameManager.Instance.StopBGM();
                    UIOperationUtil.GoToScene(MyGameManager.Instance.uiJumpData.roomMenu);
                }
                else
                {
                    GameObject.Find("UIPanel").GetComponent<RoomUIPanel>()
                        .ListUpdate();
                }

                return;
            }

            if (responseSynchronousData.Contains("status", StringComparison.Ordinal))
            {
                if (MyGameManager.CompareScene(MyGameManager.Instance.uiJumpData.roomMenu))
                {
                    var playerRoomStatusData =
                        JsonConvert.DeserializeObject<PlayerRoomStatusData>(responseSynchronousData);
                    GameObject.Find("UIPanel").GetComponent<RoomUIPanel>()
                        .CheckGameStart(playerRoomStatusData);
                }
            }
        }

        public void ResponseErrorWork(ResponseUtil responseError)
        {
            if (!_requestPool.TryGetValue(responseError.RequestID, out _))
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