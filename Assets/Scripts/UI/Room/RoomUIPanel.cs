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
                if (MyGameManager.Instance.PlayerListData.PlayerList[i].Account ==
                    MyGameManager.Instance.LocalPlayerInfo.Account)
                {
                    _localPlayerIndex = i;
                }

                _playerList.AddItemFromPool("ui://Room/PlayerListItem");
                _playerList.GetChildAt(i).asCom.GetChild("Text_playerInfo").asTextField
                    .SetVar("accountName", MyGameManager.Instance.PlayerListData.PlayerList[i].AccountName)
                    .SetVar("account", MyGameManager.Instance.PlayerListData.PlayerList[i].Account).FlushVars();
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

        public void ChangePlayerState(PlayerRoomStatusData playerRoomStatusData)
        {
            foreach (var targetPlayerReadyText in from playerItem in _playerList._children
                     where playerItem.asCom.GetChild("Text_playerInfo").asTextField.templateVars["account"] ==
                           playerRoomStatusData.Account
                     select playerItem.asCom.GetChild("Text_Ready").asTextField)
            {
                if (playerRoomStatusData.Account != MyGameManager.Instance.LocalPlayerInfo.Account)
                {
                    targetPlayerReadyText.text = targetPlayerReadyText.text == "已准备" ? "未准备" : "已准备";
                }

                return;
            }

            var gameStart = (from playerItem in _playerList._children select playerItem.asCom.GetChild("Text_Ready").asTextField).All(targetPlayerReadyText => targetPlayerReadyText.text != "未准备");

            if (gameStart)
            {
                GameStart();
            }
        }

        public void GameStart()
        {
        }
    }
}