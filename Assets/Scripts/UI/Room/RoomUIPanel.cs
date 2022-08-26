using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FairyGUI;
using Manager;
using MyWebSocket.Request;
using MyWebSocket.Response;
using UI.Util;
using UnityEngine;

namespace UI.Room
{
    public class RoomUIPanel : UIPanelUtil
    {
        private GButton _roomUIBackButton;
        private GButton _readyButton;

        private GList _playerList;

        private int _localPlayerIndex;

        protected override void Awake()
        {
            base.Awake();
            _playerList = UIRoot.GetChild("List_Player").asList;
        }

        private void Start()
        {
            ListUpdate();

            _roomUIBackButton = GetButton("Button_Back");
            _readyButton = GetButton("Button_Ready");

            _roomUIBackButton.onClick.Add(PlayerExitRoom);
            _readyButton.onClick.Add(PlayerReady);
        }

        public void ListUpdate()
        {
            _playerList.RemoveChildrenToPool();
            for (var i = 0; i < MyGameManager.Instance.PlayerListData.PlayerList.Count; i++)
            {
                var playerListNode = MyGameManager.Instance.PlayerListData.PlayerList[i];

                if (playerListNode.Account == MyGameManager.Instance.LocalPlayerInfo.Account)
                {
                    _localPlayerIndex = i;
                }

                _playerList.AddItemFromPool("ui://Room/PlayerListItem");
                _playerList.GetChildAt(i).asCom.GetChild("Text_playerInfo").asTextField
                    .SetVar("accountName", playerListNode.AccountName)
                    .SetVar("account", playerListNode.Account).FlushVars();
                _playerList.GetChildAt(i).asCom.GetChild("Text_Ready").asTextField.text = playerListNode.Status == "READY" ? "已准备" : "未准备";
            }
        }

        private void PlayerReady()
        {
            if (!MyGameManager.Instance.LocalPlayerInfo.ReadyForGame)
            {
                MyGameManager.Instance.LocalPlayerInfo.ReadyForGame = true;
                _readyButton.text = "已准备";
                _roomUIBackButton.touchable = false;
                _roomUIBackButton.GetChild("icon").asImage.color = new Color(0.4f, 0.4f, 0.4f);
                MyGameManager.Instance.NetWorkOperations.SendRequest(new RequestReady(
                    MyGameManager.Instance.LocalPlayerInfo.Account, MyGameManager.Instance.LocalPlayerInfo.GroupId));
            }
            else
            {
                MyGameManager.Instance.LocalPlayerInfo.ReadyForGame = false;
                _readyButton.text = "准备";
                _roomUIBackButton.touchable = true;
                _roomUIBackButton.GetChild("icon").asImage.color = Color.white;
                MyGameManager.Instance.NetWorkOperations.SendRequest(new RequestCancelReady(
                    MyGameManager.Instance.LocalPlayerInfo.Account, MyGameManager.Instance.LocalPlayerInfo.GroupId));
            }

            ChangeLocalPlayerReadyState();
        }

        private void ChangeLocalPlayerReadyState()
        {
            var localPlayerItemReadyText =
                _playerList.GetChildAt(_localPlayerIndex).asCom.GetChild("Text_Ready").asTextField;

            localPlayerItemReadyText.text = MyGameManager.Instance.LocalPlayerInfo.ReadyForGame ? "已准备" : "未准备";
        }

        private static void PlayerExitRoom()
        {
            var requestExitGroup = new RequestExitGroup(
                MyGameManager.Instance.LocalPlayerInfo.Account, MyGameManager.Instance.LocalPlayerInfo.GroupId);
            MyGameManager.Instance.NetWorkOperations.SendRequest(requestExitGroup);
            UIOperationUtil.GoToScene(MyGameManager.Instance.uiJumpData.bootMenu);
        }

        public void CheckGameStart(PlayerRoomStatusData playerRoomStatusData)
        {
            foreach (var playerListNode in MyGameManager.Instance.PlayerListData.PlayerList.Where(playerListNode => playerListNode.Account == playerRoomStatusData.Account))
            {
                playerListNode.Status = playerRoomStatusData.Status;
                ListUpdate();
                break;
            }

            var gameStart = MyGameManager.Instance.PlayerListData.PlayerList.All(playerListNode => playerListNode.Status == "READY");

            if (gameStart)
            {
                MyGameManager.Instance.GameStart();
            }
        }
    }
}