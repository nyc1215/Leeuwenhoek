using System;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using Manager;
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
            _readyButton.onClick.Add(PlayerReady);
        }

        private void ListUpdate()
        {
            _playerList.RemoveChildrenToPool();

            for (var i = 0; i < MyGameManager.Instance.PlayerListData.PlayerList.Count; i++)
            {
                if (MyGameManager.Instance.PlayerListData.PlayerList[i].Account == MyGameManager.Instance.account)
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
            if (!MyGameManager.Instance.readyForGame)
            {
                MyGameManager.Instance.readyForGame = true;
                _readyButton.text = "已准备";
                _roomUIBackButton.touchable = false;
                _roomUIBackButton.GetChild("icon").asImage.color = new Color(0.4f, 0.4f, 0.4f);
            }
            else
            {
                MyGameManager.Instance.readyForGame = false;
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

            localPlayerItemReadyText.text = MyGameManager.Instance.readyForGame ? "已准备" : "未准备";
        }
    }
}